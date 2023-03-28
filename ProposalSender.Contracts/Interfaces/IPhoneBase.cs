using System.Collections.ObjectModel;

namespace ProposalSender.Contracts.Interfaces
{
    public interface IPhoneBase
    {
        ObservableCollection<long> Phones { get; set; }
        void DeletePhone(long number);
        void AddOnePhoneNumber(long number);
        void DeleAllPhones(IEnumerable<long> phones);
    }
}
