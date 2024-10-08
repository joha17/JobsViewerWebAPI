using System;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JobsViewer_API.Models
{
    public class DecryptConnectionString
    {

        private readonly IConfiguration _configuration;
        private static readonly HttpClient client = new HttpClient();

        public DecryptConnectionString(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> DecryptConnectionAsync(string connectionstring)
        {
            var url = _configuration["APIEncryptDecrypt:url"] + "/" + "Desencriptar"; // URL del API
            string appkey = _configuration["AppKey:Key"];
            var data = new
            {
                appKey = appkey,
                dato = connectionstring
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            else
            {
                return null;
            }
        }
    }
}
