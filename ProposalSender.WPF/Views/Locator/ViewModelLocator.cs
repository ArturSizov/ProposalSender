using ProposalSender.Contracts.Implementations;
using ProposalSender.WPF.Infrastructure;
using ProposalSender.WPF.ViewModels;
using Unity;

namespace ProposalSender.WPF.Views.Locator
{
    public class ViewModelLocator
    {
        public TelegramSenderWindowViewModel TelegramSenderWindowViewModel => RootContainer.Container.Resolve<TelegramSenderWindowViewModel>();
    }
}
