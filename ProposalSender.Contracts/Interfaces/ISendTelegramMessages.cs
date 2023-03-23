using ProposalSender.Contracts.Models;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ISendTelegramMessages
    {
        Task Connect(UserSender user);
        Task SendCode(UserSender user);
        Task DoLogin(string loginInfo);
        Task SendMessage(IEnumerable<long> users, string message = "App Send Telegram Messages");
        string LoginInfo { get; set; }

        //UserSender UserSender { get; set; }
        //List<long> Phones { get; set; }
        //Task SendMessage(string message);
        //string ErrorMessage { get; set; }
        //string LoginInfo { get; set; }
        //bool VerificationView { get; set; }
    }
}
