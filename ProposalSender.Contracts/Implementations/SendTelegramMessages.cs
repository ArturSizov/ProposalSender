using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using TL;
using WTelegram;

namespace ProposalSender.Contracts.Implementations
{
    public class SendTelegramMessages : ISendTelegramMessages
    {
        #region Private proprty
        private Client? client;
        #endregion

        #region Public property
        public string LoginInfo { get; set; }
        public string Status { get; set; }
        public string InfoMessage { get; set; }
        public bool IsEnabled { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Connect method
        /// </summary>
        /// <param name="user"></param>
        /// <param name="verificationValue"></param>
        /// <returns></returns>
        public async Task<Client> Connect(UserSender user, string verificationValue)
        {
            try
            {
                IsEnabled = true;

                if (client == null)
                    client = new Client(Convert.ToInt32(user.ApiId), user.ApiHash);

                await DoLogin(verificationValue);

                return client;
            }
            catch
            {
                IsEnabled = false;
                InfoMessage = "Не верный API HASH";
                return null;
            }
        }

        /// <summary>
        /// Disconnect method
        /// </summary>
        public void Disconnect()
        {
            client?.Reset(true, true);
            IsEnabled = false;
            Status = string.Empty;
        }

        /// <summary>
        /// Message sending method
        /// </summary>
        /// <param name="message"></param>
        public async Task SendMessage(UserSender user, IEnumerable<long>users, string message = "App Send Telegram Messages")
        {
            int countSent = 0;
            int countUnsent = 0;

            try
            {
                if (client?.UserId == 0)
                    return;

                else
                {
                    foreach (var item in users)
                    {
                        if (client != null)
                        {
                            var result = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = $"+7{item}" } });
                            if (result.users.Count != 0)
                            {
                                var mes = await client.SendMessageAsync(result.users[result.imported[0].user_id], $"{message}");
                                countSent++;
                            }
                            else countUnsent++;
                            
                        }
                    }
                    InfoMessage = $"Количество отправленных сообщений: {countSent}\nНомера не пользуются Telegram: {countUnsent}";
                }
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "You must connect to Telegram first":
                        InfoMessage = "Нет подключения к Telegram";
                        break;
                    default:
                        InfoMessage = ex.Message;
                        break;
                }
            }
        }

        private async Task DoLogin(string loginInfo)
        {
            try
            {
                string what = await client.Login(loginInfo); 

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
                            LoginInfo = string.Empty;
                            break;
                    }
                }
                else
                {
                    LoginInfo = string.Empty;
                    Status = $"Подключено как {client.User}";
                    IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "You must provide a config value for phone_number":
                        InfoMessage = "Не указан номер телефона";
                        break;
                    case "Сделана попытка выполнить операцию на сокете для недоступного хоста.":
                        InfoMessage = "Нет подключения к Интернету";
                        break;
                    case "API_ID_INVALID":
                        InfoMessage = "Не верный API ID";
                        break;
                    case "PHONE_CODE_INVALID":
                        InfoMessage = "Не верный код верификации";
                        break;
                    case "FLOOD_WAIT_X":
                        InfoMessage = "Аккаунт временно заблокирован";
                        break;
                    case "PHONE_NUMBER_INVALID":
                        InfoMessage = "Не верный номер телефона";
                        break;
                    case "PHONE_NUMBER_BANNED":
                        InfoMessage = "Номер телефона заблокирован";
                        break;
                    default:
                        InfoMessage = ex.Message;
                        break;
                }

                Disconnect();
            }
        }
        #endregion
    }
}
