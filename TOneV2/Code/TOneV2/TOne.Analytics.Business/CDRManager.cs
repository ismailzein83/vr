using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class CDRManager
    {
        private readonly ICDRDataManager _datamanager;
        public CDRManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<ICDRDataManager>();
        }
        public CDRBigResult GetCDRData(string tempTableKey, CDRFilter filter, DateTime fromDate, DateTime toDate, int fromRow, int toRow, int nRecords, BillingCDROptionMeasures CDROption, BillingCDRMeasures orderBy, bool isDescending)
        {

            return _datamanager.GetCDRData(tempTableKey, filter, fromDate, toDate, fromRow, toRow, nRecords, CDROption, orderBy, isDescending);
        }

        public HttpResponseMessage ExportCDRData(CDRBigResult records)
        {
            Workbook wbk = new Workbook();
            Worksheet RateWorkSheet = wbk.Worksheets.Add("Top Entity Report");
            int Irow = 1;
            int Icol = 0;
            var headers = Enum.GetNames(typeof(BillingCDRMeasures));
            foreach (string header in headers)
            {
                Irow = 1;
                Icol++;
                RateWorkSheet.Cells.SetColumnWidth(Icol, 20);
                RateWorkSheet.Cells[Irow, Icol].PutValue(header);
                foreach (BillingCDR record in records.Data){
                    Irow++;
                   RateWorkSheet.Cells[Irow, Icol].PutValue(record.GetType().GetProperty(header.ToString()).GetValue(record));    
                }
            }



            for (int i = 1; i <= headers.Length ; i++)
            {
                Cell cell = RateWorkSheet.Cells.GetCell(1, i);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
            }
            wbk.Save("D:\\CDRData.xls");

            byte[] array;
            MemoryStream ms = new MemoryStream();
            ms = wbk.SaveToStream();
            array = ms.ToArray();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            ms.Position = 0;
            result.Content = new StreamContent(ms);

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "CDRData.xls"
            };

            return result;
        }

    }

}
