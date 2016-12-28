using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Aspose.Cells;
using Retail.Ringo.Entities;
using Vanrise.Entities;

namespace Retail.Ringo.Business
{
    public class RingoReportSheetManager
    {
        public Stream GenerateMNPReport(RingoReportFilter filter)
        {
            Vanrise.Common.Utilities.ActivateAspose();
            RingoMessageManager ringoMessageManager = new RingoMessageManager();

            string physicalFilePath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MNPReportPath"]);

            Workbook wbk = new Workbook(physicalFilePath);

            UpdateCellValue(wbk, 0, 3, 1, ringoMessageManager.GetRingoMessageTotal(filter.FromDate, filter.ToDate));
            UpdateCellValue(wbk, 0, 6, 1, ringoMessageManager.GetRingoMessageTotal(filter.FromDate.AddMonths(-1), filter.ToDate.AddMonths(-1)));

            UpdateWorkSheetByColumn(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                MessageTypes = new List<int>() { 1 },
                RecipientNetwork = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 2, 13, 1, 170, 168);

            UpdateWorkSheetByColumn(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                MessageTypes = new List<int>() { 1 },
                RecipientNetwork = "ICSI",
                From = filter.FromDate.AddMonths(-1),
                To = filter.ToDate.AddMonths(-1)
            }), wbk, sheetIndex: 3, rowIndex: 13, columnIndex: 1, valueColumn: 170, count: 168);


            UpdateWorkSheetByRow(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 1, 8 },
                RecipientNetwork = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 4, 11, 3, 18, 168);


            UpdateWorkSheetByRow(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 1, 8 },
                SenderNetwork = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 5, 11, 3, 16, 168);


            UpdateWorkSheetByColumn(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                MessageTypes = new List<int>() { 6 },
                SenderNetwork = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 8, 13, 1, 170, 168);


            UpdateWorkSheetByColumn(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                MessageTypes = new List<int>() { 6 },
                RecipientNetwork = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 9, 13, 1, 170, 168);

            UpdateWorkSheetByColumn(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                RecipientNetwork = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 12, 13, 1, 170, 168);

            return wbk.SaveToStream();
        }

        #region Private Methods
        void UpdateWorkSheetByRow(Dictionary<string, RingoMessageCountEntity> data, Workbook wbk, int sheetIndex, int rowIndex, int columnIndex, int valueRow, int count)
        {

            for (int i = columnIndex; i < count + columnIndex; i++)
            {
                var cellValue = wbk.Worksheets[sheetIndex].Cells[rowIndex, i].Value as string;
                RingoMessageCountEntity entity;
                if (data.TryGetValue(cellValue, out entity))
                    wbk.Worksheets[sheetIndex].Cells[valueRow, i].Value = entity.Value;
            }
        }
        void UpdateWorkSheetByColumn(Dictionary<string, RingoMessageCountEntity> data, Workbook wbk, int sheetIndex, int rowIndex, int columnIndex, int valueColumn, int count)
        {
            for (int i = rowIndex; i < count + rowIndex; i++)
            {
                var cellValue = wbk.Worksheets[sheetIndex].Cells[i, columnIndex].Value as string;
                RingoMessageCountEntity entity;
                if (data.TryGetValue(cellValue as string, out entity))
                    wbk.Worksheets[sheetIndex].Cells[i, valueColumn].Value = entity.Value;
            }
        }
        void UpdateCellValue(Workbook wbk, int sheet, int row, int column, object value)
        {
            wbk.Worksheets[sheet].Cells[row, column].Value = value;
        }

        #endregion
    }
}
