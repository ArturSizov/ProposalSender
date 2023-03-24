using ProposalSender.Contracts.Models;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ISendTelegramMessages
    {
        Task Connect(UserSender user, string verificationValue);
        Task SendMessage(IEnumerable<long> users, string message = "App Send Telegram Messages");
        string Status { get; set; }
        string LoginInfo { get; set; }
    }
}
