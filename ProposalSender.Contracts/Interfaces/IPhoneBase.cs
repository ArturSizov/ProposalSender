using System.Collections.ObjectModel;

namespace ProposalSender.Contracts.Interfaces
{
    public interface IPhoneBase
    {
        ObservableCollection<long> LoadingFromFile(string filePath);
    }
}
