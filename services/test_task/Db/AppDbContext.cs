using Microsoft.EntityFrameworkCore;
using test_task.Models;
using test_task.Services.DataProviderService;

namespace test_task.Db
{
    public class AppDbContext : DbContext
    {
        public DbSet<Resident> Residents { get; set; } = null!;

        private readonly IDataProvider _dataProvider;
        public AppDbContext(DbContextOptions<AppDbContext> options, IDataProvider dataProvider) : base(options)
        {
            _dataProvider = dataProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var task = _dataProvider.GetResidentsAsync();
            task.Wait();

            var residents = task.Result;

            modelBuilder.Entity<Resident>().HasData(residents);
        }
    }
}
