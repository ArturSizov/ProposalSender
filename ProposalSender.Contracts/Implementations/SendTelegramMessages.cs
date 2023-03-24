using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using System.Diagnostics.SymbolStore;
using TL;
using WTelegram;

namespace ProposalSender.Contracts.Implementations
{
    public class SendTelegramMessages : ISendTelegramMessages
    {
        #region Private proprty
        private Client? client;
        #endregion

        public string LoginInfo { get; set; }
        public string Status { get; set; }

        #region Methods
        public async Task Connect(UserSender user, string verificationValue)
        {
            //client?.Dispose();

            if(client == null)
                client = new Client(Convert.ToInt32(user.ApiId), user.ApiHash);

            await DoLogin(verificationValue);
        }

        
        public async Task SendCode(string verificationValue)
        {
            await DoLogin(verificationValue);
        }

        /// <summary>
        /// Message sending method
        /// </summary>
        /// <param name="message"></param>
        public async Task SendMessage(IEnumerable<long>users, string message = "App Send Telegram Messages")
        {
            foreach (var item in users)
            {
                var result = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = $"+7{item}" } });
                await client.SendMessageAsync(result.users[result.imported[0].user_id], $"{message}");
            }
        }

        private async Task DoLogin(string loginInfo)
        {
            string what = await client.Login(loginInfo);

            //var what = "verification_code";

            if (what != null)
            {
                switch (what)
                {
                    case "verification_code":
                        LoginInfo = "Код верификации:";
                        break;
                    case "password":
                        LoginInfo = "Пароль аккаунта:";
                        break;
                    default:
                        LoginInfo = null;
                        break;
                }
            }
            else
            {
                LoginInfo = null;
                Status = $"Подключено как клиент: {client.User}";
            }
        }
        #endregion
    }
}
