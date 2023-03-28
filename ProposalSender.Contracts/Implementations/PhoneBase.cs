﻿using Prism.Mvvm;
using ProposalSender.Contracts.Interfaces;
using System.Collections.ObjectModel;

namespace ProposalSender.Contracts.Implementations
{
    public class PhoneBase : BindableBase, IPhoneBase
    {
        #region Public property
        private ObservableCollection<long> phones = new();
        #endregion

        #region Public property
        public ObservableCollection<long> Phones { get => phones; set => SetProperty(ref phones, value); }
        #endregion

        public PhoneBase()
        {
            Phones.Add(9393921255);
            Phones.Add(9393997595);
        }

        #region Methods

        /// <summary>
        /// Delete one number
        /// </summary>
        /// <param name="number"></param>
        public void DeletePhone(long number)
        {
            Phones.Remove(number);
        }
        /// <summary>
        /// Delete all phone numbers
        /// </summary>
        /// <param name="phones"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void DeleAllPhones(IEnumerable<long> phones)
        {
            Phones.Clear();
        }
        #endregion
    }
}