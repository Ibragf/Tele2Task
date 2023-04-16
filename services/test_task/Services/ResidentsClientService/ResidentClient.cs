using test_task.Services.ResidentsClient.ResidentsClient;

namespace test_task.Services.ResidentsClientService
{
    public class ResidentsClient : IResidentsClient
    {
        private readonly HttpClient _client;
        public ResidentsClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetResidentsAsync()
        {
            var response = await _client.GetAsync("task");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetResidentByIdAsync(string id)
        {
            var response = await _client.GetAsync($"task/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
