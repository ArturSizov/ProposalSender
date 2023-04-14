using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows;
using ProposalSender.Contracts.Models;
using Prism.Mvvm;
using ProposalSender.Contracts.Interfaces;
using Microsoft.Win32;

namespace ProposalSender.WPF_ASP.ViewModels
{
    public class TelegramSenderWindowViewModel : BindableBase
    {
        #region Private property
        private ITMHttpClient client;
        private readonly ISendTelegramMessages send;
        private IPhoneBase phoneBase;
        private Visibility verificationView = Visibility.Collapsed;
        private string message = "Введите текст сообщения...";
        private string loginInfo;
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

        public TelegramSenderWindowViewModel(ITMHttpClient client, ISendTelegramMessages send, IPhoneBase phoneBase)
        {
            this.client = client;
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
            var result = await client.Connect(User, $"+7{User.PhoneNumber}");
            SetProperties((result.isEnabled, result.loginInfo, result.infoMessage, result.status));
            SaveProperties();
        });

        /// <summary>
        /// Disconnect command
        /// </summary>
        public ICommand Disconnect => new DelegateCommand(() =>
        {
            var result = client.Disconnect();
            Phones.Clear();
            Message = "Введите текст сообщения...";
            SelectedIndex = 0;
            //SetProperties((result.Enabled, string.Empty, string.Empty, result.Status));
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
        public ICommand SendCode => new DelegateCommand<string>(async (str) =>
        {
            await client.Connect(User, VerificationValue);
            VerificationValue = string.Empty;
            //SetProperties((result.TaskIsEnabled, result.TaskLoginnInfo, result.TaskInfoMessage, result.TaskStatus));

        }, (str) => !string.IsNullOrWhiteSpace(str));

        /// <summary>
        /// Send Message command
        /// </summary>
        public ICommand SendMessage => new DelegateCommand(async () =>
        {
            if (PingInternet())
            {
                int countSent = 0;
                int countUnsent = 0;

                foreach (var phone in Phones)
                {
                   //var result = await client.SendMessage(phone, Message);

                    //if(result.TaskErrorMessage != string.Empty)
                    //{
                    //    MessageBox.Show(result.TaskErrorMessage, "Telegram", MessageBoxButton.OK, MessageBoxImage.Error);
                    //    return;
                    //}

                    //if (result.TaskIsSend)
                    //    countSent++;
                    //else
                    //    countUnsent++;
                }
                MessageBox.Show($"Количество отправленных сообщений: {countSent}\nНомера не пользуются Telegram: {countUnsent}", "Telegram", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                client.Disconnect();
                MessageBox.Show("Нет подключения к Интернету", "Telegram", MessageBoxButton.OK, MessageBoxImage.Error);
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
        public ICommand AddOnePhoneNumber => new DelegateCommand(() =>
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
        private void SetProperties((bool isEnabled, string loginnInfo, string infoMessage, string status) result, MessageBoxImage mesImage = MessageBoxImage.Error)
        {
            IsEnabled = result.isEnabled;
            LoginInfo = result.loginnInfo;
            InfoMessage = result.infoMessage;
            Status = result.status;

            if (LoginInfo != string.Empty && IsEnabled)
                VerificationView = Visibility.Visible;
            else VerificationView = Visibility.Collapsed;

            if (InfoMessage != null && InfoMessage != string.Empty)
            {
                MessageBox.Show(InfoMessage, "Telegram", MessageBoxButton.OK, mesImage);
                InfoMessage = null;
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
