using ProposalSender.Contracts.Models;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ISendTelegramMessages
    {
        Task<(bool isEnabled, string loginnInfo, string infoMessage, string status)> ConnectAsync(UserSender? user, string verificationValue);
        Task<(bool isSend, string errorMessage)> SendMessageAsync(long phone, string message = "App Send Telegram Messages");
        Task<(string status, bool enabled)> DisconnectAsync();
    }
}
