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
        public async Task<(bool TaskIsEnabled, string TaskLoginnInfo, string TaskInfoMessage, string TaskStatus)> 
            Connect(UserSender? user, string verificationValue)
        {
            string taskLoginnInfo = string.Empty;
            string taskInfoMessage = string.Empty;
            string taskStatus = string.Empty;
            bool taskIsEnabled = false;
            try
            {
                client ??= new Client(Convert.ToInt32(user?.ApiId), user?.ApiHash);

                var result =  await DoLogin(verificationValue);

                taskStatus = result.TaskStatus;
                taskIsEnabled = result.TaskIsEnabled;
                taskInfoMessage = result.TaskInfoMessage;
                taskLoginnInfo = result.TaskLoginnInfo;

                return (taskIsEnabled, taskInfoMessage, taskLoginnInfo, taskStatus);
            }
            catch
            {
                taskIsEnabled = false;
                taskInfoMessage = "Не верный API HASH";
                return (taskIsEnabled, taskInfoMessage, taskLoginnInfo, taskStatus);
            }
        }

        /// <summary>
        /// Disconnect method
        /// </summary>
        public (string Status, bool Enabled) Disconnect()
        {
            client?.Reset(true, true);
            return (string.Empty, false);
        }

        /// <summary>
        /// Message sending method
        /// </summary>
        /// <param name="message"></param>
        public async Task<(bool TaskIsSend, string TaskErrorMessage)> SendMessage(long phone, string message = "App Send Telegram Messages")
        {
            string taskErrorMessage = string.Empty;
            bool taskIsSend = false;

            try
            {
                var result = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = $"+7{phone}" } });

                if (result.users.Count != 0)
                {
                    await client.SendMessageAsync(result.users[result.imported[0].user_id], $"{message}");
                    taskIsSend = true;
                }
                else
                    taskIsSend = false;
            }

            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "You must connect to Telegram first":
                        taskErrorMessage = "Нет подключения к Telegram";
                        break;
                    default:
                        taskErrorMessage = ex.Message;
                        break;
                }
            }
            return (taskIsSend, taskErrorMessage);
        }

        private async Task<(bool TaskIsEnabled, string TaskLoginnInfo, string TaskInfoMessage, string TaskStatus)> DoLogin(string loginInfo)
        {
            string taskLoginnInfo = string.Empty;
            string taskInfoMessage = string.Empty;
            string taskStatus = string.Empty;
            bool taskIsEnabled = true;
            try
            {
                string what = await client.Login(loginInfo); 

                if (what != null)
                {
                    switch (what)
                    {
                        case "verification_code":
                            taskLoginnInfo =  "Код верификации:";
                            break;
                        case "password":
                            taskLoginnInfo = "Пароль аккаунта:";
                            break;
                        default:
                            taskLoginnInfo = string.Empty;
                            break;
                    }
                }
                else
                {
                    taskInfoMessage = string.Empty;
                    taskStatus = $"Подключено как {client.User}";
                    taskIsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                taskInfoMessage = ex.Message switch
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
                Disconnect();
            }
            return (taskIsEnabled, taskInfoMessage, taskLoginnInfo, taskStatus);
        }
        #endregion
    }
}
