using ProposalSender.Contracts.Models;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ISendTelegramMessages
    {
        Task<(bool TaskIsEnabled, string TaskLoginnInfo, string TaskInfoMessage, string TaskStatus)> Connect(UserSender? user, string verificationValue);
        Task<(bool TaskIsSend, string TaskErrorMessage)> SendMessage(long phone, string message = "App Send Telegram Messages");
        (string Status, bool Enabled) Disconnect();
    }
}
