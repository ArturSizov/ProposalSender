using ProposalSender.Contracts.Models;

namespace ProposalSender.Contracts.Interfaces
{
    public interface ISendTelegramMessages
    {
        UserSender UserSender { get; set; }
        List<long> Phones { get; set; }
        Task SendMessage(string message);
    }
}
