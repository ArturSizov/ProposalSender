﻿using ProposalSender.Contracts.Interfaces;
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

        public string LoginInfo { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsEnabled { get; set; }

        #region Methods
        public async Task Connect(UserSender user, string verificationValue)
        {
            try
            {
                if (client == null)
                    client = new Client(Convert.ToInt32(user.ApiId), user.ApiHash);

                await DoLogin(verificationValue);
            }
            catch
            {
                ErrorMessage = "Не верный API HASH";
            }        
        }

        public void Disconnect()
        {
            if (client != null)
            {
                client.Reset(true, true);
                Status = "Не подключено";
            }
                
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
            if (client?.UserId == 0)
            {
                return;
            }
            else
            {
                foreach (var item in users)
                {
                    if(client != null)
                    {
                        var result = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = $"+7{item}" } });
                        await client.SendMessageAsync(result.users[result.imported[0].user_id], $"{message}");
                    }
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
                    Status = "Не подключено";

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
                    Status = $"Подключено как {client.User}";
                }
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "API_ID_INVALID":
                        ErrorMessage = "Не верный API ID";
                        break;
                    case "FLOOD_WAIT_X":
                        ErrorMessage = "Аккаунт временно заблокирован";
                        break;
                    case "PHONE_NUMBER_INVALID":
                        ErrorMessage = "Не верный номер телефона";
                        break;
                    case "PHONE_NUMBER_BANNED":
                        ErrorMessage = "Номер телефона заблокирован";
                        break;
                    default:
                        ErrorMessage = ex.Message;
                        break;
                }
                client.Dispose();
                client = null;
                Status = "Не подключено";
            }

        }
        #endregion
    }
}
