using Prism.Commands;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Prism.Mvvm;
using TL;
using System.Windows.Documents;
using System.Collections.ObjectModel;

namespace ProposalSender.WPF.ViewModels
{
    public class TelegramSenderWindowViewModel : BindableBase
    {
        #region Private property
        private readonly ISendTelegramMessages send;
        private Visibility verificationView = Visibility.Collapsed;
        private string message;
        private string loginInfo;
        #endregion

        #region Public property
        public string Title => "Рассылатель сообщений в Телеграм";
        public ObservableCollection<long> Phones { get; set; } = new();

        public UserSender User { get; set; } = new();

        public string Message { get => message; set => SetProperty(ref message, value); }
        public string LoginInfo { get => loginInfo; set => SetProperty(ref loginInfo, value); }
        public Visibility VerificationView { get => verificationView; set => SetProperty(ref verificationView, value); }
       
        #endregion
        public TelegramSenderWindowViewModel(ISendTelegramMessages send)
        {
            this.send = send;

            Phones.Add(9393921255);

            User = new UserSender
            {
                PhoneNumber = Properties.Settings.Default.PhoneNumber,
                ApiHash = Properties.Settings.Default.ApiHash,
                ApiId = Properties.Settings.Default.ApiId
            };

            Message = "Hello World!";
        }

        #region Commands
        /// <summary>
        /// Open link in browser command
        /// </summary>
        public ICommand OpenLink => new DelegateCommand(() =>
        {
            OpenUrl("https://my.telegram.org/auth?to=apps");
        });

        /// <summary>
        /// Connect to Telegramm command
        /// </summary>
        public ICommand ConnectTelegram => new DelegateCommand(() =>
        {
            send.SendCode(User); 
        });

        /// <summary>
        /// Send Message command
        /// </summary>
        public ICommand SendMessage => new DelegateCommand(async() =>
        {
            SaveProperties();
            if (!string.IsNullOrEmpty(Message))
            {
                await send.Connect(User);
                //await send.SendMessage(Phones, Message);
                SetProperties();
            }
        });
        #endregion

        #region Methods
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private void SetProperties()
        {
            LoginInfo = send.LoginInfo;
            if (LoginInfo != null)
                VerificationView = Visibility.Visible;
            else VerificationView = Visibility.Collapsed;
        }

        private void SaveProperties()
        {
            Properties.Settings.Default.ApiHash = User.ApiHash;
            Properties.Settings.Default.ApiId = User.ApiId;
            Properties.Settings.Default.PhoneNumber = User.PhoneNumber;
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}
