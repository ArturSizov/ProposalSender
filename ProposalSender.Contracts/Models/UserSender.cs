namespace ProposalSender.Contracts.Models
{
    public class UserSender
    {
        public long PhoneNumber { get; set; }
        public string? ApiId { get; set; }
        public string? ApiHash { get; set; }
    }
}
