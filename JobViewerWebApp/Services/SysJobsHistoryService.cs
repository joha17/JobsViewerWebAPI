using JobViewerWebApp.Models;

namespace JobViewerWebApp.Services
{
    public class SysJobsHistoryService
    {
        private readonly HttpClient _httpClient;

        public SysJobsHistoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("JobsApiClient");
        }

        public async Task<List<SysJobHistory>> GetSysJobsHistoryAsync()
        {
            try
            {
                var sysjobhistory = await _httpClient.GetFromJsonAsync<List<SysJobHistory>>("api/sysjobhistory");
                return sysjobhistory;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<List<SysJobHistory>> GetSysJobsHistoryByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/sysjobhistory/{id.ToString()}");

                if (response.IsSuccessStatusCode)
                {
                    var sysjobhistory = await response.Content.ReadFromJsonAsync<List<SysJobHistory>>();
                    return sysjobhistory;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new List<SysJobHistory>();
                }
                else
                {
                    return new List<SysJobHistory>();
                }
                
                
            }
            catch (Exception)
            {
                return new List<SysJobHistory>();
                throw;
            }
            
        }
        public async Task<List<SysJobHistory>> GetDistinctServersAsync()
        {
            try
            {
                var sysjobhistory = await _httpClient.GetFromJsonAsync<List<SysJobHistory>>("api/sysjobhistory");

                var distinctServers = sysjobhistory.Distinct(new JobHistoryServerComparer()).ToList();
                return (distinctServers);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
