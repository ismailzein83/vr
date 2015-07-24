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
    public class TrafficStatisticManager
    { 
        public TrafficStatisticSummaryBigResult GetTrafficStatisticSummary(string tempTableKey, TrafficStatisticFilter filter, bool withSummary, TrafficStatisticGroupKeys[] groupKeys, DateTime from, DateTime to, int fromRow, int toRow, TrafficStatisticMeasures orderBy, bool isDescending)
        {
            ITrafficStatisticDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            return dataManager.GetTrafficStatisticSummary(tempTableKey, filter, withSummary, groupKeys, from, to, fromRow, toRow, orderBy, isDescending);
        }

        public IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to)
        {
            ITrafficStatisticDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            return dataManager.GetTrafficStatistics(filterByColumn, columnFilterValue, from, to);
        }

        public HttpResponseMessage ExportTrafficStatisticSummary(TrafficStatisticSummaryBigResult records, List<string> headers, TrafficStatisticGroupKeys[] groupKeys)
        {
            Workbook wbk = new Workbook();
            wbk.Worksheets.Clear();
            Worksheet RateWorkSheet = wbk.Worksheets.Add("Traffic Statistic");
            int Irow = 1;
            int Icol = 0;
           // var headers = Enum.GetNames(typeof(TrafficStatisticMeasures));
            foreach(TrafficStatisticGroupKeys groupKey in groupKeys){
                Irow = 1;
                Icol++;
                RateWorkSheet.Cells.SetColumnWidth(Icol, 20);
                RateWorkSheet.Cells[Irow, Icol].PutValue(groupKey.GetType().GetEnumName(groupKey));
                foreach (TrafficStatisticGroupSummary record in records.Data)
                {
                    Irow++;
                    RateWorkSheet.Cells[Irow, Icol].PutValue(record.GroupKeyValues[Icol-1].Name);
                }

             }
            foreach (string header in headers)
            {
                Irow = 1;
                Icol++;
                RateWorkSheet.Cells.SetColumnWidth(Icol, 20);
                RateWorkSheet.Cells[Irow, Icol].PutValue(header);
                foreach (TrafficStatisticGroupSummary record in records.Data)
                {
                    Irow++;
                    RateWorkSheet.Cells[Irow, Icol].PutValue(record.GetType().GetProperty(header.ToString()).GetValue(record));
                }
            }

            for (int i = 1; i <= groupKeys.Length; i++)
            {
                Cell cell = RateWorkSheet.Cells.GetCell(1, i);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
            }

            for (int i = 1; i <= headers.Count; i++)
            {
                Cell cell = RateWorkSheet.Cells.GetCell(1, groupKeys.Length+i);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
            }
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
                FileName = "TrafficStatistic-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + ".xls"
            };

            return result;
        }
    }
}
