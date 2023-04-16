using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using test_task.Controllers;
using test_task.Models;
using test_task.Db;
using Microsoft.AspNetCore.Mvc;
using test_task.Services.DataProviderService;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using test_task.Tests.Models;

namespace test_task.Tests
{
    public class ResidentsControllerTests
    {
        private Fixture _fixture = new Fixture();
        private string[] _genders = new[] { "male", "female" };

        private AppDbContext _mockContext;
        private ResidentsController _controller;
        private List<Resident> _residents;

        [SetUp]
        public void SetUp()
        {
            Random rand = new Random();
            var residents = _fixture.CreateMany<Resident>(30);
            foreach (var res in residents)
            {
                res.Sex = _genders[rand.Next(0, _genders.Length)];
                res.Age = rand.Next(0, 151);
            }
            _residents = residents.ToList();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("test_db")
                .Options;

            var mockProvider = new Mock<IDataProvider>();
            mockProvider.Setup(m => m.GetResidentsAsync()).ReturnsAsync(residents);

            _mockContext = new AppDbContext(options, mockProvider.Object);
            _mockContext.Residents.AddRange(residents);

            _mockContext.SaveChanges();

            _controller = new ResidentsController(_mockContext);
        }

        [TestCase(1, 0, 12, "mle")]
        [TestCase(1, 0, 12, "")]
        public async Task GIVEN_ResidentsController_WHEN_GetResidents_method_is_invoked_THEN_bad_request_is_returned(
            int? page,
            int? ageFrom,
            int? ageTo,
            string? sex)
        {

            //Act
            var result = await _controller.GetResidentsAsync(page, ageFrom, ageTo, sex);

            //Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var errors = badRequestResult.Value as SerializableError;
            Assert.NotNull(errors);
        }

        [TestCase(1, 10, 30, "male")]
        [TestCase(3, 0, 100, "female")]
        [TestCase(2, 0, 20, "male")]
        [TestCase(4, 10, 40, null)]
        [TestCase(null, 20, 50, null)]
        [TestCase(null, 20, null, null)]
        [TestCase(null, null, null, null)]
        public async Task GIVEN_ResidentsController_WHEN_GetResidents_method_is_invoked_THEN_correct_value_is_returned(
            int? page,
            int? ageFrom,
            int? ageTo,
            string? sex)
        {
            //Arrange
            page = page ?? 1;

            var query = _mockContext.Residents
                .Where(x => x.Age >= ageFrom && x.Age <= ageTo);

            if (!string.IsNullOrEmpty(sex)) query = query.Where(x => x.Sex == sex);
            
            var expectedList = query
                .Skip((page.Value - 1) * 3)
                .Take(3)
                .Select(resident => new Resident { Id=resident.Id, Name=resident.Name, Sex=resident.Sex })
                .ToList();

            var expected = new ResidentsResponse
            {
                CurrentPage = page.Value,
                Pages = (int)Math.Ceiling(query.Count() / 3f),
                Residents = expectedList
            };

            //Act
            var result = await _controller.GetResidentsAsync(page, ageFrom, ageTo, sex);

            //Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var response = (ResidentsResponse) okResult.Value;
            var actual = expected.Residents.ExceptBy(response.Residents.Select( x=> x.Id), x=> x.Id).Count();

            Assert.That(actual, Is.EqualTo(0));
        }

        [TestCase(30,0,1,null)]
        public async Task GIVEN_ResidentsController_WHEN_GetResidents_method_is_invoked_THEN_not_found_is_returned(
            int? page,
            int? ageFrom,
            int? ageTo,
            string? sex)
        {
            //Act
            var result = await _controller.GetResidentsAsync(page, ageFrom, ageTo, sex);

            //Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

            [Test]
        public async Task GIVEN_ResidentsController_WHEN_GetResidentsById_method_is_invoked_THEN_correct_value_is_returned()
        {
            //Arrange
            Random r = new Random();
            var id = _residents[r.Next(0, _residents.Count+1)].Id;
            var expected = _mockContext.Residents.Where(x => x.Id == id).First();

            //Act
            var result = await _controller.GetResidentByIdAsync(id);

            //Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            Assert.That(JsonSerializer.Serialize(okResult.Value),
                Is.EqualTo(JsonSerializer.Serialize(expected)));
        }

        [Test]
        public async Task GIVEN_ResidentsController_WHEN_GetResidentsById_method_is_invoked_THEN_not_found_is_returned()
        {
            //Arrange
            var id = "3";

            // Act
            var result = await _controller.GetResidentByIdAsync(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
