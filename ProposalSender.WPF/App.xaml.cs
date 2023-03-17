using ProposalSender.Contracts.Implementations;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.WPF.Infrastructure;
using ProposalSender.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
        }
    }
}
