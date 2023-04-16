using test_task.Models;

namespace test_task.Services.DataProviderService
{
    public interface IDataProvider
    {
        Task<IEnumerable<Resident>> GetResidentsAsync();
    }
}
