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

        #region Methods
        /// <summary>
        /// Connect method
        /// </summary>
        /// <param name="user"></param>
        /// <param name="verificationValue"></param>
        /// <returns></returns>
        public async Task<(bool isEnabled, string loginInfo, string infoMessage, string status)>
            ConnectAsync(UserSender? user, string verificationValue)
        {
            string loginInfo = string.Empty;
            string infoMessage;
            string status = string.Empty;
            bool isEnabled = false;
            try
            {
                client ??= new Client(Convert.ToInt32(user?.ApiId), user?.ApiHash);

                var result =  await DoLoginAsync(verificationValue);

                status = result.status;
                isEnabled = result.isEnabled;
                infoMessage = result.infoMessage;
                loginInfo = result.loginInfo;

                return (isEnabled, loginInfo, infoMessage, status);
            }
            catch(Exception ex)
            {
                isEnabled = false;
                infoMessage = ex.Message;
                return (isEnabled, loginInfo, infoMessage, status);
            }
        }

        /// <summary>
        /// Disconnect method
        /// </summary>
        public async Task<(string status, bool enabled)> DisconnectAsync()
        {
            await Task.Run(()=> client?.Reset());
            return (string.Empty, false);
        }

        /// <summary>
        /// Message sending method
        /// </summary>
        /// <param name="message"></param>
        public async Task<(bool isSend, string errorMessage)> SendMessageAsync(long phone, string message = "App Send Telegram Messages")
        {
            string errorMessage = string.Empty;
            bool isSend = false;

            try
            {
                var result = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = $"+7{phone}" } });

                if (result.users.Count != 0)
                {
                    await client.SendMessageAsync(result.users[result.imported[0].user_id], $"{message}");
                    isSend = true;
                }
                else
                    isSend = false;
            }

            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "You must connect to Telegram first":
                        errorMessage = "Нет подключения к Telegram";
                        break;
                    default:
                        errorMessage = ex.Message;
                        break;
                }
            }
            return (isSend, errorMessage);
        }

        private async Task<(bool isEnabled, string loginInfo, string infoMessage, string status)> DoLoginAsync(string loginInfo)
        {
            string infoMessage = string.Empty;
            string status = string.Empty;
            bool isEnabled = true;
            try
            {
                string what = await client.Login(loginInfo); 

                if (what != null)
                {
                    switch (what)
                    {
                        case "verification_code":
                            loginInfo =  "Код верификации:";
                            break;
                        case "password":
                            loginInfo = "Пароль аккаунта:";
                            break;
                        default:
                            loginInfo = string.Empty;
                            break;
                    }
                }
                else
                {
                    status = $"Подключено как {client.User}";
                    loginInfo = string.Empty;
                    isEnabled = true;
                }
            }
            catch (Exception ex)
            {
                infoMessage = ex.Message switch
                {
                    "You must provide a config value for phone_number" => "Не указан номер телефона",
                    "Сделана попытка выполнить операцию на сокете для недоступного хоста." => "Нет подключения к Интернету",
                    "API_ID_INVALID" => "Не верный API ID",
                    "PHONE_CODE_INVALID" => "Не верный код верификации",
                    "FLOOD_WAIT_X" => "Аккаунт временно заблокирован",
                    "PHONE_NUMBER_INVALID" => "Не верный номер телефона",
                    "PHONE_NUMBER_BANNED" => "Номер телефона заблокирован",
                    _ => ex.Message,
                };
                loginInfo = string.Empty;
                isEnabled = false;
                await DisconnectAsync();
            }
            return (isEnabled, loginInfo, infoMessage, status);
        }
        #endregion


    }
}
