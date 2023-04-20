using ProposalSender.Contracts.Models;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ITMHttpClient
    {
        string LoginInfo { get; set; }
        string Status { get; set; }
        public bool IsEnabled { get; set; }
        string InfoMessage { get; set; }
        Task<(bool isEnabled, string loginInfo, string infoMessage, string status)> ConnectAsync(UserSender? user, string verificationValue);
        Task<(bool isEnabled, string loginInfo, string infoMessage, string status)> SenCodeAsync(string verificationValue);
        Task SendMessageAsync(long phone, string message);
        Task<(string status, bool isEnabled)> DisconnectAsync();
    }
}
