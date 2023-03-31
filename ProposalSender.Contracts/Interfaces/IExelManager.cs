using System.Collections.ObjectModel;

namespace ProposalSender.Contracts.Interfaces
{
    public interface IExelManager
    {
        ObservableCollection<long> ReadExelFile(string filePath);
    }
}
