using ProposalSender.Contracts.Implementations;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.WPF_ASP.Infrastructure;
using ProposalSender.WPF_ASP.ViewModels;
using System.Windows;
using Unity;

namespace ProposalSender.WPF_ASP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureIOC();
        }

        private void ConfigureIOC()
        {
            RootContainer.Container.RegisterSingleton<TelegramSenderWindowViewModel, TelegramSenderWindowViewModel>();
            RootContainer.Container.RegisterSingleton<ISendTelegramMessages, SendTelegramMessages>();
            RootContainer.Container.RegisterSingleton<IPhoneBase, PhoneBase>();
            RootContainer.Container.RegisterSingleton<IExelManager, ExelManager>();
        }
    }
}
