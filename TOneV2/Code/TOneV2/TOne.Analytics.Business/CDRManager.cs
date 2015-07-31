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
        public Vanrise.Entities.IDataRetrievalResult<BillingCDR> GetCDRData(Vanrise.Entities.DataRetrievalInput<CDRSummaryInput> input)
        {

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _datamanager.GetCDRData(input)); 
        }

        public HttpResponseMessage ExportCDRData( CDRBigResult records)
        {
            //Aspose.Cells.License license = new Aspose.Cells.License();
            //license.SetLicense("Aspose.Cells.lic");
            
            //CDRBigResult records = _datamanager.GetCDRData(tempTableKey, nRecords);
            Workbook wbk = new Workbook();
            wbk.Worksheets.Clear();
            Worksheet RateWorkSheet = wbk.Worksheets.Add("CDR Log");

            wbk.Worksheets.ActiveSheetIndex = 1;
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
           // wbk.Save("D:\\CDRData.xls");

            byte[] array;
            MemoryStream ms = new MemoryStream();
            ms = wbk.SaveToStream();
            array = ms.ToArray();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            ms.Position = 0;
            result.Content = new StreamContent(ms);

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
           
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "CDRData-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-"+DateTime.Now.Day.ToString() + ".xls"
            };

            return result;
        }

    }

}
