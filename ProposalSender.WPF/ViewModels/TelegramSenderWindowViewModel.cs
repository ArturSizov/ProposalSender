using Prism.Commands;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ProposalSender.WPF.ViewModels
{
    public class TelegramSenderWindowViewModel : BindableBase
    {
        #region Private property
        private readonly ISendTelegramMessages send;
        private Visibility verificationView = Visibility.Collapsed;
        private string message = "Введите текст сообщения...";
        private string loginInfo;
        private string verificationValue;
        private string status;
        private string errorMessage;
        private bool isEnabled = false;
        #endregion

        #region Public property
        public string Title => "Рассылатель в Telegram";
        public ObservableCollection<long> Phones { get; set; } = new();
        public UserSender User { get; set; } = new();
        public string Message { get => message; set => SetProperty(ref message, value); }
        public string VerificationValue { get => verificationValue; set => SetProperty(ref verificationValue, value); }
        public string LoginInfo { get => loginInfo; set => SetProperty(ref loginInfo, value); }
        public string Status { get => status; set => SetProperty(ref status, value); }
        public string ErrorMessage { get => errorMessage; set => SetProperty(ref errorMessage, value); }
        public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }
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

            //Connect.Execute(null);
        }

        #region Commands
        public ICommand Connect => new DelegateCommand<string>(async(str) =>
        {
            await send.Connect(User, $"+7{User.PhoneNumber}");
            SetProperties();
            SaveProperties();
        });

        public ICommand Disconnect => new DelegateCommand(() =>
        {
            send.Disconnect();
            SetProperties();
        });
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
        public ICommand SendCode => new DelegateCommand<string>(async(str) =>
        {
            await send.Connect(User, VerificationValue);
            SetProperties();
            VerificationValue = string.Empty;

        },(str)=> !string.IsNullOrWhiteSpace(str));

        /// <summary>
        /// Send Message command
        /// </summary>
        public ICommand SendMessage => new DelegateCommand<string>(async(str) =>
        {
            await send.SendMessage(Phones, Message);

        },(str)=> !string.IsNullOrWhiteSpace(str) & IsEnabled);
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
           Status = send.Status;

           LoginInfo = send.LoginInfo;

           IsEnabled = send.IsEnabled;

           if (LoginInfo != string.Empty)
                VerificationView = Visibility.Visible;
           else VerificationView = Visibility.Collapsed;

            if (send.ErrorMessage != null)
            {
                MessageBox.Show(send.ErrorMessage, "Telegram", MessageBoxButton.OK, MessageBoxImage.Error);
                send.ErrorMessage = string.Empty;
            }
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
