using JobViewerWebApp.Models;
using System.Linq;

namespace JobViewerWebApp.Services
{
    public class SysJobsService
    {
        private readonly HttpClient _httpClient;

        public SysJobsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("JobsApiClient");
        }

        public async Task<List<SysJob>> GetSysJobsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<SysJob>>("api/sysjobs");
        }

        public async Task<SysJob> GetProductoByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<SysJob>($"api/sysjobs/{id}");
        }

        public async Task<List<SysJob>> GetDistinctServersAsync()
        {
            try
            {
                var sysjobs = await _httpClient.GetFromJsonAsync<List<SysJob>>("api/sysjobs");

                var distinctServers = sysjobs.Distinct(new JobsServerComparer()).ToList();
                return (distinctServers);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
