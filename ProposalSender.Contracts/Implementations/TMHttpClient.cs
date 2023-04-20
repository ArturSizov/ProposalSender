using Newtonsoft.Json;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using System.Net.Http.Headers;
using System.Text;

namespace ProposalSender.Contracts.Implementations
{
    public class TMHttpClient : ITMHttpClient
    {
        #region Private property
        private HttpClient client;
        private string url = "https://localhost:7154/phones";
        #endregion

        #region Piblic property
        public string LoginInfo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string InfoMessage { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        #endregion

        public TMHttpClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
        }

        #region Methods
        /// <summary>
        /// Connection method to Telegtam
        /// </summary>
        /// <param name="user"></param>
        /// <param name="verificationValue"></param>
        /// <returns></returns>
        public async Task<(bool isEnabled, string loginInfo, string infoMessage, string status)> ConnectAsync(UserSender? user, string verificationValue)
        {
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{url}/connect", data).Result.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<(bool, string, string, string)>(response);
            return (result.Item1, result.Item2, result.Item3, result.Item4);
        }

        /// <summary>
        /// Verification code entry method in Telegram
        /// </summary>
        /// <param name="verificationValue"></param>
        /// <returns></returns>
        public async Task<(bool isEnabled, string loginInfo, string infoMessage, string status)> SenCodeAsync(string verificationValue)
        {
            var response = await client.PostAsync($"{url}/sendcode?verificationValue={verificationValue}", null).Result.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<(bool, string, string, string)>(response);
            return (result.Item1, result.Item2, result.Item3, result.Item4);
        }

        public async Task SendMessageAsync(long phone, string message)
        {
            await client.PostAsync($"{url}/sendmessage?phone={phone}&message={message}", null);
        }

        /// <summary>
        /// Shutdown method Telegramm
        /// </summary>
        /// <returns></returns>
        public async Task<(string status, bool isEnabled)> DisconnectAsync()
        {
            var response = await client.PostAsync($"{url}/disconnect", null).Result.Content.ReadAsStringAsync(); ;
            var result = JsonConvert.DeserializeObject<(string, bool)>(response);
            return (result.Item1, result.Item2);
        }
        #endregion
    }
}
