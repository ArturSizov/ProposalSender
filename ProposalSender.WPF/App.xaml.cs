using Prism.Ioc;
using ProposalSender.Contracts.Implementations;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.WPF.Views;
using System.Windows;

namespace ProposalSender.WPF
{
    public partial class App
    {
        protected override Window CreateShell() => Container.Resolve<TelegramSenderWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISendTelegramMessages, SendTelegramMessages>();
            containerRegistry.Register<IPhoneBase, PhoneBase>();
            containerRegistry.Register<IExelManager, ExelManager>();
        }
    }
}
