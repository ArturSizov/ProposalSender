using ProposalSender.Contracts.Implementations;
using ProposalSender.Contracts.Interfaces;

namespace ProposalSender.WPF.ViewModels
{
    public class TelegramSenderWindowViewModel
    {
        private readonly ISendTelegramMessages send;

        public string Title => "Рассылатель сообщений в Телеграм";

        public TelegramSenderWindowViewModel(ISendTelegramMessages send)
        {
            this.send = send;

            send.Phones.Add(9393921255);

            send.UserSender = new Contracts.Models.UserSender
            {
                PhoneNumber = 9393910200,
                Name = "Артур",
                LastName = "Сизов",
                Password = "3355413",
                ApiHash = "23398527f241060a3ac9da0fca3a68f8",
                ApiId = "28077592",
                VerificationCode = "22398"
            };

            send.SendMessage("App Send Telegram Messages");
        }


    }
}
