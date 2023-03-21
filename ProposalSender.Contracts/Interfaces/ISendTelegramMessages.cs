using ProposalSender.Contracts.Models;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ISendTelegramMessages
    {
        Task Connect();
        Task SendCode();
        UserSender UserSender { get; set; }
        List<long> Phones { get; set; }
        Task SendMessage(string message);
        string ErrorMessage { get; set; }
        string LoginInfo { get; set; }
        bool VerificationView { get; set; }
    }
}
