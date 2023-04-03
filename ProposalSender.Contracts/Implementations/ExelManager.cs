using ProposalSender.Contracts.Interfaces;
using OfficeOpenXml;
using System.Collections.ObjectModel; 

namespace ProposalSender.Contracts.Implementations
{
    public class ExelManager : IExelManager
    {
        /// <summary>
        /// Method for reading from excel file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ObservableCollection<long> ReadExelFile(string filePath)
        {
            var phones = new ObservableCollection<long>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var excelFile = new ExcelPackage(new FileInfo(filePath));

            var worksheet = excelFile.Workbook.Worksheets[0];

            var totalRows = worksheet.Dimension.End.Row;

            var value = worksheet.Cells[$"A1:A{totalRows}"].ToList();

            foreach (var item in value)
            {
                var res = phones.Contains(long.Parse(item.Text));

                if (item.Text.Length == 10 && !phones.Contains(long.Parse(item.Text)))
                {
                   phones.Add(long.Parse(item.Text));
                }
            }


            if (phones.Count > 200)
                throw new ArgumentException("Количество добавляемых номеров не может быть больше 200.\nПоправьте список в exel файле");

            return phones;
        }
    }
}
