namespace ProposalSender.Contracts.Models
{
    public class UserSender
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public long PhoneNumber { get; set; }
        public string ApiId { get; set; }
        public string ApiHash { get; set; }
        public string VerificationCode { get; set; }
        public string Password { get; set; }
    }
}
