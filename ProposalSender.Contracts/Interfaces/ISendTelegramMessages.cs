using ProposalSender.Contracts.Models;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ISendTelegramMessages
    {
        Task Connect(UserSender user, string verificationValue);
        Task SendMessage(UserSender user, IEnumerable<long> users, string message = "App Send Telegram Messages");
        void Disconnect();
        string LoginInfo { get; set; }
        string Status { get; set; }
        public bool IsEnabled { get; set; }
        string InfoMessage { get; set; }
    }
}
