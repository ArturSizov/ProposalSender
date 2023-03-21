using Prism.Commands;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Prism.Mvvm;
using TL;

namespace ProposalSender.WPF.ViewModels
{
    public class TelegramSenderWindowViewModel : BindableBase
    {
        #region Private property
        private readonly ISendTelegramMessages send;
        private bool verificationView = false;
        private string message;
        private string loginInfo;
        #endregion

        #region Public property
        public string Title => "Рассылатель сообщений в Телеграм";

        public UserSender User { get; set; } = new();

        public string Message { get => message; set => SetProperty(ref message, value); }
        public string LoginInfo { get => loginInfo; set => SetProperty(ref loginInfo, value); }
        public bool VerificationView { get => verificationView; set => SetProperty(ref verificationView, value); }
        #endregion
        public TelegramSenderWindowViewModel(ISendTelegramMessages send)
        {
            this.send = send;

            send.Phones.Add(9393921255);

            send.UserSender = new UserSender
            {
                PhoneNumber = 9393910200,
                Name = "Артур",
                LastName = "Сизов",
                ApiHash = "23398527f241060a3ac9da0fca3a68f8",
                ApiId = "28077592"
            };

            User = send.UserSender;

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
            send.SendCode();

            Message = send.LoginInfo;

            VerificationView = send.VerificationView;

            LoginInfo = send.LoginInfo;

            if (send.ErrorMessage != null)
                MessageBox.Show($"{send.ErrorMessage}"); 
        });

        /// <summary>
        /// Send Message command
        /// </summary>
        public ICommand SendMessage => new DelegateCommand(() =>
        {
            if(!string.IsNullOrEmpty(Message)) send.SendMessage(Message);
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
            #endregion
        }
    }
}
