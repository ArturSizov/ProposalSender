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

            var value = worksheet.Cells[$"Q23:Q{totalRows}"].ToList();

            foreach (var item in value)
            {
                phones.Add(long.Parse(item.Text)); 
            }

            return phones;
        }
    }
}
