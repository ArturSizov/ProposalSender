using ProposalSender.Contracts.Implementations;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.WPF.Infrastructure;
using ProposalSender.WPF.ViewModels;
using System.Windows;
using Unity;

namespace ProposalSender.WPF
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
            RootContainer.Container.RegisterSingleton<IExelManager, ExelManager>();
            RootContainer.Container.RegisterSingleton<IPhoneBase, PhoneBase>();
        }
    }
}
