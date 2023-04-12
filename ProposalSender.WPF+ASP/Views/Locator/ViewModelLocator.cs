using ProposalSender.WPF_ASP.Infrastructure;
using ProposalSender.WPF_ASP.ViewModels;
using Unity;

namespace ProposalSender.WPF_ASP.Views.Locator
{
    public class ViewModelLocator
    {
        public TelegramSenderWindowViewModel TelegramSenderWindowViewModel => RootContainer.Container.Resolve<TelegramSenderWindowViewModel>(); 
    }
}
