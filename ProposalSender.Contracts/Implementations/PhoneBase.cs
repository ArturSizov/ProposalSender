using Prism.Mvvm;
using ProposalSender.Contracts.Interfaces;
using System.Collections.ObjectModel;

namespace ProposalSender.Contracts.Implementations
{
    public class PhoneBase : BindableBase, IPhoneBase
    {
        #region Private property
        private readonly IExelManager exel;
        #endregion

        public PhoneBase(IExelManager exel)
        {
            this.exel = exel;
        }

        #region Methods
        /// <summary>
        /// Download method from excel file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        ObservableCollection<long> IPhoneBase.LoadingFromFile(string filePath)
        {
            return exel.ReadExelFile(filePath);
        }
        #endregion
    }
}
