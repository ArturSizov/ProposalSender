using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using TL;
using WTelegram;

namespace ProposalSender.Contracts.Implementations
{
    public class SendTelegramMessages : ISendTelegramMessages
    {
        private Client client;

        #region Public property
        public UserSender UserSender { get; set; } = new();
        public List<long> Phones { get; set; } = new();
        #endregion
        public SendTelegramMessages()
        {
            UserSender = new UserSender
            {
                PhoneNumber = 9393910200,
                Name = "Артур",
                LastName = "Сизов",
                Password = "3355413",
                ApiHash = "23398527f241060a3ac9da0fca3a68f8",
                ApiId = "28077592"
            };
            //Phones.Add(9393921255);
            client = new Client(Config);
            //Task.Run(async () => await client.LoginUserIfNeeded());

        }

        #region Methods
        /// <summary>
        /// Configuration for WTelegram.Client
        /// </summary>
        /// <param name="what"></param>
        /// <returns></returns>
        private string? Config(string what)
        {
            switch (what)
            {
                case "api_id": return $"{UserSender.ApiId}";
                case "api_hash": return $"{UserSender.ApiHash}";
                case "phone_number": return $"+7{UserSender.PhoneNumber}";
                case "verification_code": Console.Write("Code: ");
                    return Console.ReadLine();
                case "first_name": return $"{UserSender.Name}";      // if sign-up is required
                case "last_name": return $"{UserSender.LastName}";        // if sign-up is required
                case "password": return $"{UserSender.Password}";     // if user has enabled 2FA
                default: return null;                  // let WTelegramClient decide the default config
            }
        }
        /// <summary>
        /// Message sending method
        /// </summary>
        /// <param name="message"></param>
        public async Task  SendMessage(string message = "App Send Telegram Messages")
        {
            await client.LoginUserIfNeeded();
            foreach (var item in Phones)
            {
                var result = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = $"+7{item}" } });
                await client.SendMessageAsync(result.users[result.imported[0].user_id], $"{message}");
            }
        }

        #endregion
    }
}
