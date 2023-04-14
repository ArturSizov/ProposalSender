using ProposalSender.Contracts.Models;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ITMHttpClient
    {
        string LoginInfo { get; set; }
        string Status { get; set; }
        public bool IsEnabled { get; set; }
        string InfoMessage { get; set; }
        Task<(bool isEnabled, string loginInfo, string infoMessage, string status)> Connect(UserSender? user, string verificationValue);
        Task SenCode(string verificationValue);
        Task SendMessage(long phone, string message);
        Task Disconnect();
    }
}
