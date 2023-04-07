using Prism.Commands;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System;
using ProposalSender.Contracts.Implementations;

namespace ProposalSender.WPF.ViewModels
{
    public class TelegramSenderWindowViewModel : BindableBase
    {
        #region Private property
        private readonly ISendTelegramMessages send;
        private readonly IPhoneBase phoneBase;
        private Visibility verificationView = Visibility.Collapsed;
        private string message = "Введите текст сообщения...";
        private string loginInfo = string.Empty;
        private string verificationValue;
        private string status;
        private string infoMessage;
        private bool isEnabled = false;
        private ObservableCollection<long> phones = new();
        private bool installationStatusApp;
        private int selectedIndex = 0;
        #endregion

        #region Public property
        public string Title => "Рассылатель в Telegram";
        public ObservableCollection<long> Phones { get => phones; set => SetProperty(ref phones, value); }
        public UserSender User { get; set; } = new();
        public string Message { get => message; set => SetProperty(ref message, value); }
        public string VerificationValue { get => verificationValue; set => SetProperty(ref verificationValue, value); }
        public string LoginInfo { get => loginInfo; set => SetProperty(ref loginInfo, value); }
        public string Status { get => status; set => SetProperty(ref status, value); }
        public string InfoMessage { get => infoMessage; set => SetProperty(ref infoMessage, value); }
        public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }
        public Visibility VerificationView { get => verificationView; set => SetProperty(ref verificationView, value); }
        public bool InstallationStatusApp { get => installationStatusApp; set => SetProperty(ref installationStatusApp, value); }
        public int SelectedIndex { get => selectedIndex; set => SetProperty(ref selectedIndex, value); }

        #endregion

        public TelegramSenderWindowViewModel(ISendTelegramMessages send, IPhoneBase phoneBase)
        {
            this.send = send;
            this.phoneBase = phoneBase;

            User = new UserSender
            {
                ApiHash = Properties.Settings.Default.ApiHash,
                ApiId = Properties.Settings.Default.ApiId,
                PhoneNumber = Properties.Settings.Default.PhoneNumber
            };
        }

        #region Commands
        /// <summary>
        /// Connect command
        /// </summary>
        public ICommand Connect => new DelegateCommand<string>(async(str) =>
        {
            await send.Connect(User, $"+7{User.PhoneNumber}");
            SetProperties();
            SaveProperties();
        });

        /// <summary>
        /// Disconnect command
        /// </summary>
        public ICommand Disconnect => new DelegateCommand(() =>
        {
            send.Disconnect();
            send.IsEnabled = false;
            Phones.Clear();
            Message = "Введите текст сообщения...";
            SelectedIndex = 0;
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
        public ICommand SendMessage => new DelegateCommand(async() =>
        {
            if (PingInternet())
            {
                int countSent = 0;
                int countUnsent = 0;

                foreach (var phone in Phones)
                {
                    var res = await send.SendMessage(phone, Message);

                    if(res)
                        countSent++;
                    else 
                        countUnsent++;
                }
                send.InfoMessage = $"Количество отправленных сообщений: {countSent}\nНомера не пользуются Telegram: {countUnsent}";

                SetProperties(MessageBoxImage.Information);
            }
            else
            {
                send.Disconnect();
                send.InfoMessage = "Нет подключения к Интернету";
                SetProperties(MessageBoxImage.Error);
            }
        });

        /// <summary>
        /// Delete one phone number command
        /// </summary>
        public ICommand DeleteOnePhone => new DelegateCommand<object>((obj) =>
        {
            Phones.Remove((long)obj); 
            RaisePropertyChanged(nameof(Phones));
        });

        /// <summary>
        /// Delete all phone numbers command
        /// </summary>
        public ICommand DeletAllPhones => new DelegateCommand(() =>
        {   
            Phones.Clear();
            RaisePropertyChanged(nameof(Phones));
        });

        /// <summary>
        /// Add one phone number command
        /// </summary>
        public ICommand AddOnePhoneNumber => new DelegateCommand(()=>
        {
            Phones.Add(9393806425);
        });

        /// <summary>
        /// Download command from excel file
        /// </summary>
        public ICommand LoadingFromFile => new DelegateCommand(() =>
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Exel|*.xlsx";

            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    var filePath = openFileDialog.FileName;
                    Phones = phoneBase.LoadingFromFile(filePath);
                }
                else return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Telegram", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void SetProperties(MessageBoxImage mesImage = MessageBoxImage.Error)
        {
           Status = send.Status;

           LoginInfo = send.LoginInfo;

           IsEnabled = send.IsEnabled;

            if (LoginInfo != string.Empty && IsEnabled)
                VerificationView = Visibility.Visible;
            else VerificationView = Visibility.Collapsed;

            if (send.InfoMessage != null)
            {
                MessageBox.Show(send.InfoMessage, "Telegram", MessageBoxButton.OK, mesImage);
                send.InfoMessage = null;
            }
        }
        private void SaveProperties()
        {
            Properties.Settings.Default.ApiHash = User.ApiHash;
            Properties.Settings.Default.ApiId = User.ApiId;
            Properties.Settings.Default.PhoneNumber = User.PhoneNumber;
            Properties.Settings.Default.Save();
        }
        private bool PingInternet()
        {
            bool bb = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (bb == true)
               return true;
            else
                return false;
        }
        #endregion
    }
}
