using System.Text.Json;
using test_task.Models;
using test_task.Services.DataProviderService;
using test_task.Services.ResidentsClient.ResidentsClient;

namespace test_task.Services
{
    public class DataProvider : IDataProvider
    {
        private readonly IResidentsClient _residentsClient;
        public DataProvider(IResidentsClient residentsClient)
        {
            _residentsClient = residentsClient;
        }

        public virtual async Task<IEnumerable<Resident>> GetResidentsAsync()
        {
            var jsonString = await _residentsClient.GetResidentsAsync();

            var residents = JsonSerializer.Deserialize<List<Resident>>(jsonString);

            foreach (var resident in residents)
            {
                var json = await _residentsClient.GetResidentByIdAsync(resident.Id);

                var jsonDoc = JsonDocument.Parse(json);
                var root = jsonDoc.RootElement;

                resident.Age = root.GetProperty("age").GetInt32();
            }

            return residents;
        }
    }
}
