namespace test_task.Services.ResidentsClient.ResidentsClient
{
    public interface IResidentsClient
    {
        Task<string> GetResidentsAsync();
        Task<string> GetResidentByIdAsync(string id);
    }
}
