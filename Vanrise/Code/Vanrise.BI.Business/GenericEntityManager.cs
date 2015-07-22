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
using Vanrise.BI.Data;
using Vanrise.BI.Entities;

namespace Vanrise.BI.Business
{
    public class GenericEntityManager
    {   

        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params string[] measureTypeNames)
        {

            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypeNames);
        }

        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(string entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params string[] measureTypes)
        {
            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetEntityMeasuresValues(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypes);
        }
        public IEnumerable<EntityRecord> GetTopEntities(string entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, params string[] measureTypesNames)
        {
            List<String> queryFilter = new List<String>();
            queryFilter = null;
            //queryFilter.Add("C001");
            //queryFilter.Add("C009");
            //queryFilter.Add("C020");
            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetTopEntities(entityTypeName, topByMeasureTypeName, fromDate, toDate, topCount, queryFilter, measureTypesNames);
        }

        public Decimal[] GetMeasureValues(DateTime fromDate, DateTime toDate, params string[] measureTypeNames)
        {

            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetMeasureValues(fromDate, toDate, measureTypeNames);
        }
        public HttpResponseMessage ExportMeasureValues(IEnumerable<TimeValuesRecord> records, string entity, string[] measureTypesNames, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            Workbook wbk = new Workbook();
            Worksheet RateWorkSheet = wbk.Worksheets.Add("Time Variation Report");
        
        

            int Irow = 1;
            int Icol = 1;
            RateWorkSheet.Cells[Irow, Icol].PutValue(entity);
            RateWorkSheet.Cells.SetColumnWidth(Icol, 20);
            foreach (string header in measureTypesNames)
            {
                Icol++;
                RateWorkSheet.Cells[Irow, Icol].PutValue(header);
                RateWorkSheet.Cells.SetColumnWidth(Icol, 20);
            }
            
            foreach (TimeValuesRecord record in records)
            {
                Irow++;
                Icol = 1;

                RateWorkSheet.Cells[Irow, Icol].PutValue(GetDateTimeProperties(record, timeDimensionType, fromDate, toDate, true));
                foreach (Decimal value in  record.Values)
                {
                    Icol++;
                    RateWorkSheet.Cells[Irow, Icol].PutValue(value);
                }

            }

            for (int i = 1; i <= measureTypesNames.Length+1; i++)
            {
                Cell cell = RateWorkSheet.Cells.GetCell(1, i);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
            }

            //Adding a chart to the worksheet
            int chartIndex = RateWorkSheet.Charts.Add(Aspose.Cells.Charts.ChartType.Pyramid, 5, 0, 15, 5);

            // Accessing the instance of the newly added chart
            Aspose.Cells.Charts.Chart chart = RateWorkSheet.Charts[chartIndex];

            // Adding SeriesCollection (chart data source) to the chart ranging from "A1" cell to "B3"
            chart.NSeries.Add("C3:D5", true);


            wbk.Save("D:\\book1.xls");

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
                FileName = "Data.xls"
            };

            return result;
        }
        public HttpResponseMessage ExportTopEntities(IEnumerable<EntityRecord> records, string entity, string[] measureTypesNames)
        {
            Workbook wbk = new Workbook();
            Worksheet RateWorkSheet = wbk.Worksheets.Add("Top Entity Report");
            int Irow = 1;
            int Icol = 1;
            RateWorkSheet.Cells[Irow, Icol].PutValue(entity);
            RateWorkSheet.Cells.SetColumnWidth(Icol, 20);
            foreach (string header in measureTypesNames)
            {
                Icol++;
                RateWorkSheet.Cells.SetColumnWidth(Icol, 20);
                RateWorkSheet.Cells[Irow, Icol].PutValue(header);
            }

            foreach (EntityRecord record in records)
            {
                Irow++;
                Icol = 1;
                RateWorkSheet.Cells[Irow, Icol].PutValue(record.EntityName);
                foreach (Decimal value in record.Values)
                {
                    Icol++;
                    RateWorkSheet.Cells[Irow, Icol].PutValue(value);
                }

            }

            for (int i = 1; i <= measureTypesNames.Length+1; i++)
            {
                Cell cell = RateWorkSheet.Cells.GetCell(1, i);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
            }

            //Adding a chart to the worksheet
            int chartIndex = RateWorkSheet.Charts.Add(Aspose.Cells.Charts.ChartType.Pyramid, 5, 0, 15, 5);

            // Accessing the instance of the newly added chart
            Aspose.Cells.Charts.Chart chart = RateWorkSheet.Charts[chartIndex];

            // Adding SeriesCollection (chart data source) to the chart ranging from "A1" cell to "B3"
            chart.NSeries.Add("C3:D5", true);


            wbk.Save("D:\\book1.xls");

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
                FileName = "Data.xls"
            };

            return result;
        }

        public string GetDateTimeProperties(TimeValuesRecord record, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, Boolean dontFillGroup)
        {
                
             if (dontFillGroup == false) {
                 var isLongPeriod = CheckIsLongPeriod(timeDimensionType, fromDate, toDate);
                        if (isLongPeriod == true)
                         dontFillGroup = true;
                     }

             DateTime dateTimeValue = record.Time;
                   switch (timeDimensionType) {
                       case TimeDimensionType.Yearly:
                           {
                               return dateTimeValue.Year.ToString();
                           }
                       case TimeDimensionType.Monthly:
                           {
                               if (dontFillGroup)
                                 return (GetMonthNameShort(dateTimeValue) + "-" + GetShortYear(dateTimeValue));
                               break;
                           }
                       case TimeDimensionType.Weekly:
                           {
                               var groupName = GetMonthNameShort(dateTimeValue) + "-" + GetShortYear(dateTimeValue);
                               if (dontFillGroup)
                                   return ("Week " + record.WeekNumber + "-" + groupName);
                               break;
                           }
                   
                       case TimeDimensionType.Daily:
                           {
                               var groupName = GetMonthNameShort(dateTimeValue) + "-" + GetShortYear(dateTimeValue);
                               if (dontFillGroup)
                                   return (dateTimeValue.Day.ToString() + "-" + groupName);
                               break;
                           }
                       case TimeDimensionType.Hourly:
                           {
                               string hour = dateTimeValue.Hour.ToString();
                               string minute = dateTimeValue.Minute.ToString();
                               var groupName = dateTimeValue.Date + "-" + GetMonthNameShort(dateTimeValue) + "-" + GetShortYear(dateTimeValue);
                               if (dontFillGroup)
                                 return ( groupName + " " + (hour.ToCharArray().Length < 2 ? '0' + hour : hour) + ":" + (minute.ToCharArray().Length < 2 ? '0' + minute : minute));
                               break;
                           }
                  
                        }
                   return null;
        
    }
         public Boolean CheckIsLongPeriod(TimeDimensionType timeDimensionType,DateTime fromDate, DateTime toDate) {
       
        switch (timeDimensionType) {
            case TimeDimensionType.Yearly:
                return false;
            case TimeDimensionType.Monthly:
                if (toDate.Year- fromDate.Year > 4)
                    return true;
                else
                    return false;
            case TimeDimensionType.Weekly:
                if (GetDateDifference(fromDate, toDate) > 200)
                    return true;
                else
                    return false;
            case TimeDimensionType.Daily:
                if (GetDateDifference(fromDate, toDate) > 50)
                    return true;
                else
                    return false;
            case TimeDimensionType.Hourly:
                if (GetDateDifference(fromDate, toDate) > 2)
                    return true;
                else
                    return false;
            }
        return false;
         }
         public double GetDateDifference(DateTime fromDate, DateTime toDate)
         {
             double timeDiff = toDate.Millisecond - fromDate.Millisecond;
             return Math.Ceiling(timeDiff / (1000 * 3600 * 24));
            }
            public string GetShortYear(DateTime date)
    {
        string fullYear = date.Year.ToString();
        if (fullYear.Length == 4)
            return fullYear.Substring(2);
        else
            return fullYear;
    }
           public string GetMonthNameShort(DateTime date) {
               string[] shortMonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                int monthIndex = date.Month;
               return shortMonthNames[monthIndex-1];
            }
    }
}
