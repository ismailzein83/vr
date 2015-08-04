using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Data;
using Vanrise.BI.Entities;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.BI.Business
{
    public class GenericEntityManager
    {   

        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params string[] measureTypeNames)
        {

            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            List<BIConfiguration<BIConfigurationMeasure>> allMeasures= configurations.GetMeasures();
            List<BIConfiguration<BIConfigurationEntity>> entities = configurations.GetEntities();
            dataManager.MeasureDefinitions = allMeasures;
            dataManager.EntityDefinitions = entities;
            List<String> customerIds = new List<String>();
            string customerColumnId=null;
           foreach (BIConfiguration<BIConfigurationMeasure> measure in allMeasures){
                foreach (string measureName in measureTypeNames)
                 {
                     if (measureName == measure.Name && measure.Configuration.Type == MeasureConfigurationType.Financial)
                     {
                         foreach (BIConfiguration<BIConfigurationEntity> entity in entities)
                         {
                             if (entity.Name == "Customer")
                             {
                                 customerColumnId = entity.Configuration.ColumnID;
                                 var myObject = (IDimensionBehavior)Activator.CreateInstance(Type.GetType(entity.Configuration.BehaviorFQTN));
                                 customerIds = myObject.GetFilteredValues();
                             }
                            
                         }
                     }
                 }
           }
            
            List<String> supplierIds = new List<String>();

            return dataManager.GetMeasureValues(timeDimensionType, fromDate, toDate, customerIds, supplierIds,customerColumnId, measureTypeNames);
        }

        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(string entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params string[] measureTypes)
        {
            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            List<String> supplierIds=new List<String>();
            List<String> customerIds=new List<String>();
            string customerColumnId = null;
            return dataManager.GetEntityMeasuresValues(entityType, entityId, timeDimensionType, fromDate, toDate, customerIds, supplierIds,customerColumnId, measureTypes);
        }
        public IEnumerable<EntityRecord> GetTopEntities(string entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, params string[] measureTypesNames)
        {
            List<String> queryFilter = new List<String>();
            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            List<BIConfiguration<BIConfigurationEntity>> entities = configurations.GetEntities();
          
            foreach (BIConfiguration<BIConfigurationEntity> entity in entities)
            {
                if (entityTypeName==entity.Name && entity.Configuration.BehaviorFQTN != null)
                {
                    var myObject = (IDimensionBehavior)Activator.CreateInstance(Type.GetType(entity.Configuration.BehaviorFQTN));
                    queryFilter=  myObject.GetFilteredValues();
                    
                }
                
            }
            if (queryFilter.Count == 0)
                queryFilter = null;
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = entities;
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
            wbk.Worksheets.Clear();
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
            //int chartIndex = RateWorkSheet.Charts.Add(Aspose.Cells.Charts.ChartType.Pyramid, 5, 0, 15, 5);

            // Accessing the instance of the newly added chart
            //Aspose.Cells.Charts.Chart chart = RateWorkSheet.Charts[chartIndex];

            // Adding SeriesCollection (chart data source) to the chart ranging from "A1" cell to "B3"
            //chart.NSeries.Add("C3:D5", true);

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
                FileName = "TimeVariatioReport-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + ".xls"
            };

            return result;
        }
        public HttpResponseMessage ExportTopEntities(IEnumerable<EntityRecord> records, string entity, string[] measureTypesNames)
        {
            Workbook wbk = new Workbook();
            wbk.Worksheets.Clear();
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
            //int chartIndex = RateWorkSheet.Charts.Add(Aspose.Cells.Charts.ChartType.Pyramid, 5, 0, 15, 5);

            //// Accessing the instance of the newly added chart
            //Aspose.Cells.Charts.Chart chart = RateWorkSheet.Charts[chartIndex];

            //// Adding SeriesCollection (chart data source) to the chart ranging from "A1" cell to "B3"
            //chart.NSeries.Add("C3:D5", true);


           

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
                FileName = "TopEntityReport-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + ".xls"
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
                               var groupName = dateTimeValue.Day.ToString() + "-" + GetMonthNameShort(dateTimeValue) + "-" + GetShortYear(dateTimeValue);
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


           public Vanrise.Entities.IDataRetrievalResult<UserMeasuresValidator> GetUserMeasuresValidator(Vanrise.Entities.DataRetrievalInput<UserMeasuresValidatorInput> userMeasuresValidatorInput)
        {
             // Dictionary<int,List<string>> userMeasuresValidator=new   Dictionary<int,List<string>>();
            
              List<UserMeasuresValidator> userMeasuresValidator = new List<UserMeasuresValidator>();
              List<string> distinctMeasures = new List<string>();
              WidgetsManager widgetsManager = new WidgetsManager();
              List<WidgetDetails> widgets = widgetsManager.GetAllWidgets();
              foreach(WidgetDetails widget in widgets)
              {
                 foreach (int widgetId in userMeasuresValidatorInput.Query.Widgets)
                   {
                       if (widget.Id == widgetId)
                       {
                           List<string> measures = ((BIWidgetSetting)widget.Setting.Settings).GetMeasures();
                           foreach (string measure in measures)
                           {
                               if (!distinctMeasures.Contains(measure))
                                   distinctMeasures.Add(measure);
                           }
                       }
                   }
               }
               BIConfigurationManager manager=new BIConfigurationManager();
              List<BIConfiguration<BIConfigurationMeasure>> allMeasures= manager.GetMeasures();

              List<BIConfiguration<BIConfigurationMeasure>> filteredMeasures = new List<BIConfiguration<BIConfigurationMeasure>>();
              for (int i = 0; i < allMeasures.Count; i++)
              {
                  if (distinctMeasures.Contains(allMeasures[i].Name))
                  {
                      filteredMeasures.Add(allMeasures[i]);
                  }
              }
              List<int> distinctUsers = new List<int>();
              if ((userMeasuresValidatorInput.Query.UserIds!=null && userMeasuresValidatorInput.Query.UserIds.Count != 0) || (userMeasuresValidatorInput.Query.GroupIds!=null && userMeasuresValidatorInput.Query.GroupIds.Count != 0))
              {
                  for (int i = 0; i < userMeasuresValidatorInput.Query.UserIds.Count; i++)
                  {
                      if (!distinctUsers.Contains(userMeasuresValidatorInput.Query.UserIds[i]))
                          distinctUsers.Add(userMeasuresValidatorInput.Query.UserIds[i]);
                  }
                  List<User> users = new List<User>();
                  UserManager userManager = new UserManager();
                  for (int i = 0; i < userMeasuresValidatorInput.Query.GroupIds.Count; i++)
                  {
                      users = userManager.GetMembers(userMeasuresValidatorInput.Query.GroupIds[i]);
                      foreach (User user in users)
                      {
                          if (!distinctUsers.Contains(user.UserId) && user.Status != UserStatus.Inactive)
                              distinctUsers.Add(user.UserId);

                      }
                  }
              }
              else
              {
                  List<User> users = new List<User>();
                  UserManager userManager = new UserManager();
                  users=userManager.GetUsers();
                  foreach (User user in users)
                  {
                      if (!distinctUsers.Contains(user.UserId) && user.Status != UserStatus.Inactive)
                          distinctUsers.Add(user.UserId);

                  }
              }
              
             
               
              Vanrise.Security.Business.SecurityManager securityManager = new Vanrise.Security.Business.SecurityManager();
              foreach (int user in distinctUsers)
              {
                  List<string> userDeniedMeasures = new List<string>();
                  foreach (BIConfiguration<BIConfigurationMeasure> measure in filteredMeasures)
                  {
                      if (measure.Configuration.RequiredPermissions != null && !securityManager.IsAllowed(measure.Configuration.RequiredPermissions, user))
                          userDeniedMeasures.Add(measure.Name);
                  }
                  if (userDeniedMeasures.Count>0)
                  userMeasuresValidator.Add(new UserMeasuresValidator{UserId=user,MeasuresDenied=userDeniedMeasures});
              }
              BigResult<UserMeasuresValidator> returnedData = new BigResult<UserMeasuresValidator>();
              returnedData.Data = userMeasuresValidator;
              return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(userMeasuresValidatorInput, returnedData);
             
        }
    }
}
