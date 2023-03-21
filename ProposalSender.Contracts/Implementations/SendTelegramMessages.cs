using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using TL;
using WTelegram;

namespace ProposalSender.Contracts.Implementations
{
    public class SendTelegramMessages : ISendTelegramMessages
    {
        #region Public property
        public UserSender UserSender { get; set; } = new();
        public List<long> Phones { get; set; } = new();
        public string LoginInfo { get; set; }
        public string ErrorMessage { get; set; }
        public bool VerificationView { get; set; }
        #endregion

        private Client? client; 

        #region Methods

        public async Task Connect()
        {
            client?.Dispose();

            client = new Client(Convert.ToInt32(UserSender.ApiId), UserSender.ApiHash);

            await DoLogin($"+7{UserSender.PhoneNumber}");
        }

        public async Task SendCode()
        {
            await DoLogin(UserSender.VerificationCode);
        }

        /// <summary>
        /// Message sending method
        /// </summary>
        /// <param name="message"></param>
        public async Task SendMessage(string message = "App Send Telegram Messages")
        {
            await Connect();

            foreach (var item in Phones)
            {
                var result = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = $"+7{item}" } });
                await client.SendMessageAsync(result.users[result.imported[0].user_id], $"{message}");
            }
        }

        public async Task DoLogin(string loginInfo) // (add this method to your code)
        {
            string what = await client.Login(loginInfo);

            if (what != null)
            {
                VerificationView = true;

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
        }
        #endregion
    }
}
