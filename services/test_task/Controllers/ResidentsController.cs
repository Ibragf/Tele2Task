using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using test_task.Db;
using test_task.Models;

namespace test_task.Controllers
{
    [Route("api/residents")]
    [ApiController]
    public class ResidentsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public ResidentsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetResidentsAsync([Range(1, int.MaxValue)] int? page, 
                                                           [Range(0, 150)] int? ageFrom,
                                                           [Range(0, 150)] int? ageTo,
                                                           string? sex)
        {
            if (sex != null)
            {
                if (sex != "male" && sex != "female")
                {
                    ModelState.AddModelError(nameof(sex), "The 'sex' field value could be only 'male' or 'female'.");
                    return BadRequest(ModelState);
                }
            }

            var query = _dbContext.Residents.AsQueryable();

            if(!string.IsNullOrEmpty(sex)) query = query.Where(resident => resident.Sex == sex);

            if (ageFrom.HasValue && ageTo.HasValue)
                query = query.Where(resident => resident.Age >= ageFrom && resident.Age <= ageTo);

            var pageResults = 3;
            int currentPage = page ?? 1;

            var pages = Math.Ceiling(query.Count() / (float)pageResults);
            if (currentPage > pages)
                return NotFound();

            var residents = await query
                .Skip((currentPage-1) * pageResults)
                .Take(pageResults)
                .Select(resident => new Resident(resident.Id, resident.Name, resident.Sex))
                .ToListAsync();

            var response = new ResidentsResponse
            {
                CurrentPage = currentPage,
                Pages = (int) pages,
                Residents = residents,
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetResidentByIdAsync(string id)
        {
            var resident = await _dbContext.Residents.FirstOrDefaultAsync(resident => resident.Id == id);

            if(resident == null) return NotFound();

            return Ok(resident);
        }
    }

    public class ResidentsResponse
    {
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
        public IEnumerable<Resident> Residents { get; set; }
    }
}
