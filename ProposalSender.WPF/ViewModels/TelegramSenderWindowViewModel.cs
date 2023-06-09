﻿using Prism.Commands;
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
        private string? verificationValue;
        private string? status;
        private string? infoMessage;
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
        public string VerificationValue { get => verificationValue!; set => SetProperty(ref verificationValue, value); }
        public string LoginInfo { get => loginInfo; set => SetProperty(ref loginInfo, value); }
        public string Status { get => status!; set => SetProperty(ref status, value); }
        public string InfoMessage { get => infoMessage!; set => SetProperty(ref infoMessage, value); }
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
        public ICommand ConnectCommandAsync => new DelegateCommand<string>(async(str) =>
        {
            var result = await send.ConnectAsync(User, $"+7{User.PhoneNumber}");
            SetProperties((result.isEnabled, result.loginInfo, result.infoMessage, result.status));
            SaveProperties();
        });

        /// <summary>
        /// Disconnect command
        /// </summary>
        public ICommand DisconnectCommandAsync => new DelegateCommand(async () =>
        {
            var result = await send.DisconnectAsync();
            Phones.Clear();
            Message = "Введите текст сообщения...";
            SelectedIndex = 0;
            SetProperties((result.enabled, string.Empty, string.Empty, result.status));
        });
        /// <summary>
        /// Open link in browser command
        /// </summary>
        public ICommand OpenLinkCommand => new DelegateCommand(() =>
        {
            OpenUrl("https://my.telegram.org/auth?to=apps");
        });

        /// <summary>
        /// Connect to Telegramm command
        /// </summary>
        public ICommand SendCodeCommandAsync => new DelegateCommand<string>(async(str) =>
        {
            var result = await send.ConnectAsync(User, VerificationValue);
            VerificationValue = string.Empty;
            SetProperties((result.isEnabled, result.loginInfo, result.infoMessage, result.status));

        },(str)=> !string.IsNullOrWhiteSpace(str));

        /// <summary>
        /// Send Message command
        /// </summary>
        public ICommand SendMessageCommandAsync => new DelegateCommand(async() =>
        {
            if (PingInternet())
            {
                int countSent = 0;
                int countUnsent = 0;

                foreach (var phone in Phones)
                {
                    var result = await send.SendMessageAsync(phone, Message);

                    if (result.errorMessage != string.Empty)
                    {
                        MessageBox.Show(result.errorMessage, "Telegram", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (result.isSend)
                        countSent++;
                    else
                        countUnsent++;
                }
                MessageBox.Show($"Количество отправленных сообщений: {countSent}\nНомера не пользуются Telegram: {countUnsent}", "Telegram", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await send.DisconnectAsync();
                MessageBox.Show("Нет подключения к Интернету", "Telegram", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        /// <summary>
        /// Delete one phone number command
        /// </summary>
        public ICommand DeleteOnePhoneCommand => new DelegateCommand<object>((obj) =>
        {
            Phones.Remove((long)obj); 
            RaisePropertyChanged(nameof(Phones));
        });

        /// <summary>
        /// Delete all phone numbers command
        /// </summary>
        public ICommand DeletAllPhonesCommand => new DelegateCommand(() =>
        {   
            Phones.Clear();
            RaisePropertyChanged(nameof(Phones));
        });

        /// <summary>
        /// Add one phone number command
        /// </summary>
        public ICommand AddOnePhoneNumberCommand => new DelegateCommand(()=>
        {
            Phones.Add(9393806425);
        });

        /// <summary>
        /// Download command from excel file
        /// </summary>
        public ICommand LoadingFromFileCommand => new DelegateCommand(() =>
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
                InfoMessage = string.Empty;
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

            if (bb)
               return true;
            else
                return false;
        }
        #endregion
    }
}
