using ProposalSender.Contracts.Models;
using WTelegram;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ISendTelegramMessages
    {
        Task<Client> Connect(UserSender user, string verificationValue);
        Task<bool> SendMessage(UserSender user, long phone, string message = "App Send Telegram Messages");
        void Disconnect();
        string LoginInfo { get; set; }
        string Status { get; set; }
        public bool IsEnabled { get; set; }
        string InfoMessage { get; set; }
    }
}
