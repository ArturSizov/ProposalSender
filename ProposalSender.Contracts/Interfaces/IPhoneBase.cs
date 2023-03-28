using System.Collections.ObjectModel;

namespace ProposalSender.Contracts.Interfaces
{
    public interface IPhoneBase
    {
        ObservableCollection<long> Phones { get; set; }
    }
}
