using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpgrade.Net.Client.ServerAPIs.APIMock
{
    internal class AuthAPIMock : IAuthAPI
    {
        public async Task<string> GetToken(string userName, string password)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"http://127.0.0.1:9099/api/Authenticate/token?name={userName}&password={password}&appId=Upgrader");
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<Token>(await httpResponseMessage.Content.ReadAsStringAsync()).access_token;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }
        private class Token
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }
    }
}
