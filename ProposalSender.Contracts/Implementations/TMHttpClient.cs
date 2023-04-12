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
        private ISendTelegramMessages send;
        private HttpClient client;
        private string url = "https://localhost:7154/phones";
        #endregion

        #region Piblic property
        public string LoginInfo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string InfoMessage { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        #endregion

        public TMHttpClient(ISendTelegramMessages send)
        {
            this.send = send;
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
        }

        #region Methods
        public async Task Connect(UserSender? user, string verificationValue)
        {
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync($"{url}/connect", data);
        }

        public async Task Disconnect()
        {
            await client.PostAsync($"{url}/disconnect", null);
        }

        public async Task SenCode(string verificationValue)
        {
            var json = JsonConvert.SerializeObject(verificationValue);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync($"{url}/sendcode", data);
            SetProperty();
        }

        public async Task SendMessage(long phone, string message)
        {
            await client.PostAsync($"{url}/sendmessage?phone={phone}&message={message}", null);
            SetProperty();
        }

        private void SetProperty()
        {
        }
        #endregion
    }
}
