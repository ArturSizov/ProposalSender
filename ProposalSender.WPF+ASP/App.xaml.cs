using Prism.Ioc;
using ProposalSender.Contracts.Implementations;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.WPF_ASP.Views;
using System.Windows;

namespace ProposalSender.WPF_ASP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell() => Container.Resolve<TelegramSenderWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ITMHttpClient, TMHttpClient>();
            containerRegistry.Register<IPhoneBase, PhoneBase>();
            containerRegistry.Register<IExelManager, ExelManager>();
        }
    }
}
