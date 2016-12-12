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

        #region ctor
        private BIConfigurationManager _configurations;
        public GenericEntityManager()
        {
            _configurations = new BIConfigurationManager();
        }
        #endregion

        #region Public Methods
        public IEnumerable<TimeValuesRecord> GetMeasureValues(MeasureValueInput input)
        {


            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            List<BIConfiguration<BIConfigurationMeasure>> allMeasures = _configurations.GetMeasures();
            List<BIConfiguration<BIConfigurationEntity>> entities = _configurations.GetEntities();
            dataManager.MeasureDefinitions = allMeasures;
            dataManager.EntityDefinitions = entities;
            List<object> customerIds = new List<object>();
            string customerColumnId = null;
            foreach (BIConfiguration<BIConfigurationMeasure> measure in allMeasures)
            {
                foreach (string measureName in input.MeasureTypesNames)
                {
                    if (measureName == measure.Name && measure.Configuration.Type == MeasureConfigurationType.Financial)
                    {
                        foreach (BIConfiguration<BIConfigurationEntity> entity in entities)
                        {
                            if (entity.Configuration.BehaviorFQTN != null)
                            {
                                customerColumnId = entity.Configuration.ColumnID;
                                var myObject = (IDimensionBehavior)Activator.CreateInstance(Type.GetType(entity.Configuration.BehaviorFQTN));
                                customerIds = myObject.GetFilteredValues();
                            }

                        }
                    }
                }
            }

            List<object> supplierIds = new List<object>();
            BIConfigurationTimeEntity configurationTimeEntity = GetUsedTimeEntityOrDefault(input.TimeEntityName);
            return dataManager.GetMeasureValues(input, customerIds, supplierIds, customerColumnId, configurationTimeEntity);
        }
        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(EntityMeasureValueInput input)
        {
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = _configurations.GetMeasures();
            dataManager.EntityDefinitions = _configurations.GetEntities();
            List<object> supplierIds = new List<object>();
            List<object> customerIds = new List<object>();
            string customerColumnId = null;
            BIConfigurationTimeEntity configurationTimeEntity = GetUsedTimeEntityOrDefault(input.TimeEntityName);
            return dataManager.GetEntityMeasuresValues(input, customerIds, supplierIds, customerColumnId, configurationTimeEntity);
        }
        public IEnumerable<EntityRecord> GetTopEntities(TopEntityInput input)
        {
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            List<BIConfiguration<BIConfigurationEntity>> entities = _configurations.GetEntities();

            foreach (BIConfiguration<BIConfigurationEntity> entity in entities)
            {
                foreach (string entityType in input.EntityTypeName)
                    if (entityType == entity.Name && entity.Configuration.BehaviorFQTN != null)
                    {
                        var myObject = (IDimensionBehavior)Activator.CreateInstance(Type.GetType(entity.Configuration.BehaviorFQTN));
                        List<object> data = myObject.GetFilteredValues();
                        if (data.Count > 0)
                        {
                            if (input.Filter == null)
                                input.Filter = new List<DimensionFilter>();

                            if (!input.Filter.Any(x => x.EntityName == entityType))
                            {
                                input.Filter.Add(new DimensionFilter
                                {
                                    EntityName = entity.Name,
                                    Values = data
                                });
                            }
                            else
                            {
                                input.Filter.Find(x => x.EntityName == entityType).Values.AddRange(data);
                            }
                           
                        }


                    }

            }
            dataManager.MeasureDefinitions = _configurations.GetMeasures();
            dataManager.EntityDefinitions = entities;

            BIConfigurationTimeEntity configurationTimeEntity = GetUsedTimeEntityOrDefault(input.TimeEntityName);
            return dataManager.GetTopEntities(input, configurationTimeEntity);
        }
        public Decimal[] GetSummaryMeasureValues(BaseBIInput input)
        {

            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = _configurations.GetMeasures();
            dataManager.EntityDefinitions = _configurations.GetEntities();
            BIConfigurationTimeEntity configurationTimeEntity = GetUsedTimeEntityOrDefault(input.TimeEntityName);
            return dataManager.GetSummaryMeasureValues(input.FromDate, input.ToDate, configurationTimeEntity, input.MeasureTypesNames);
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
                foreach (Decimal value in record.Values)
                {
                    Icol++;
                    RateWorkSheet.Cells[Irow, Icol].PutValue(value);
                }

            }

            for (int i = 1; i <= measureTypesNames.Length + 1; i++)
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
        public HttpResponseMessage ExportTopEntities(IEnumerable<EntityRecord> records, List<string> entities, string[] measureTypesNames)
        {
            Workbook wbk = new Workbook();
            Common.Utilities.ActivateAspose();
            wbk.Worksheets.Clear();
            Worksheet RateWorkSheet = wbk.Worksheets.Add("Top Entity Report");
            int Irow = 1;
            int Icol = 1;
            foreach (string entity in entities)
            {
                Icol++;
                RateWorkSheet.Cells.SetColumnWidth(Icol, 20);
                RateWorkSheet.Cells[Irow, Icol].PutValue(entity);
            }
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

                foreach (string name in record.EntityName)
                {
                    Icol++;
                    RateWorkSheet.Cells[Irow, Icol].PutValue(name);
                }


                foreach (Decimal value in record.Values)
                {
                    Icol++;
                    RateWorkSheet.Cells[Irow, Icol].PutValue(value);
                }

            }

            for (int i = 2; i <= measureTypesNames.Length + entities.Count() + 1; i++)
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
        public Vanrise.Entities.IDataRetrievalResult<UserMeasuresValidator> GetUserMeasuresValidator(Vanrise.Entities.DataRetrievalInput<UserMeasuresValidatorInput> userMeasuresValidatorInput)
        {
            // Dictionary<int,List<string>> userMeasuresValidator=new   Dictionary<int,List<string>>();

            UserManager userManager = new UserManager();
            IEnumerable<User> allUserInfo = userManager.GetUsers();

            List<UserMeasuresValidator> userMeasuresValidator = new List<UserMeasuresValidator>();
            List<string> distinctMeasures = new List<string>();
            WidgetsManager widgetsManager = new WidgetsManager();
            IEnumerable<WidgetDetail> widgets = widgetsManager.GetAllWidgets();
            foreach (WidgetDetail widget in widgets)
            {
                foreach (int widgetId in userMeasuresValidatorInput.Query.Widgets)
                {
                    if (widget.Entity.Id == widgetId)
                    {
                        List<string> measures = ((BIWidgetSetting)widget.Entity.Setting.Settings).GetMeasures();
                        foreach (string measure in measures)
                        {
                            if (!distinctMeasures.Contains(measure))
                                distinctMeasures.Add(measure);
                        }
                    }
                }
            }
            BIConfigurationManager manager = new BIConfigurationManager();
            List<BIConfiguration<BIConfigurationMeasure>> allMeasures = manager.GetMeasures();

            List<BIConfiguration<BIConfigurationMeasure>> filteredMeasures = new List<BIConfiguration<BIConfigurationMeasure>>();
            for (int i = 0; i < allMeasures.Count; i++)
            {
                if (distinctMeasures.Contains(allMeasures[i].Name))
                {
                    filteredMeasures.Add(allMeasures[i]);
                }
            }
            List<int> distinctUsers = new List<int>();
            if ((userMeasuresValidatorInput.Query.UserIds != null && userMeasuresValidatorInput.Query.UserIds.Count != 0) || (userMeasuresValidatorInput.Query.GroupIds != null && userMeasuresValidatorInput.Query.GroupIds.Count != 0))
            {
                for (int i = 0; i < userMeasuresValidatorInput.Query.UserIds.Count; i++)
                {
                    if (!distinctUsers.Contains(userMeasuresValidatorInput.Query.UserIds[i]))
                        distinctUsers.Add(userMeasuresValidatorInput.Query.UserIds[i]);
                }
                IEnumerable<int> users = new List<int>();
                UserGroupManager userGroupManager = new UserGroupManager();
                for (int i = 0; i < userMeasuresValidatorInput.Query.GroupIds.Count; i++)
                {
                    users = userGroupManager.GetMembers(userMeasuresValidatorInput.Query.GroupIds[i]);
                    foreach (int userId in users)
                    {
                        User user = allUserInfo.FirstOrDefault(x => x.UserId == userId);
                        if (!distinctUsers.Contains(userId) && user != null && userManager.IsUserEnable(user))
                            distinctUsers.Add(userId);
                    }
                }
            }
            else
            {
                IEnumerable<User> users = userManager.GetUsers();
                foreach (User user in users)
                {
                    if (!distinctUsers.Contains(user.UserId) && userManager.IsUserEnable(user))
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
                if (userDeniedMeasures.Count > 0)
                    userMeasuresValidator.Add(new UserMeasuresValidator { UserId = user, MeasuresDenied = userDeniedMeasures });
            }
            BigResult<UserMeasuresValidator> returnedData = new BigResult<UserMeasuresValidator>();
            returnedData.Data = userMeasuresValidator;
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(userMeasuresValidatorInput, returnedData);

        }

        #endregion

        #region Private Methods
        private string GetDateTimeProperties(TimeValuesRecord record, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, Boolean dontFillGroup)
        {

            if (dontFillGroup == false)
            {
                var isLongPeriod = CheckIsLongPeriod(timeDimensionType, fromDate, toDate);
                if (isLongPeriod == true)
                    dontFillGroup = true;
            }

            DateTime dateTimeValue = record.Time;
            switch (timeDimensionType)
            {
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
                            return (groupName + " " + (hour.ToCharArray().Length < 2 ? '0' + hour : hour) + ":" + (minute.ToCharArray().Length < 2 ? '0' + minute : minute));
                        break;
                    }

            }
            return null;

        }
        private Boolean CheckIsLongPeriod(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {

            switch (timeDimensionType)
            {
                case TimeDimensionType.Yearly:
                    return false;
                case TimeDimensionType.Monthly:
                    if (toDate.Year - fromDate.Year > 4)
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
        private double GetDateDifference(DateTime fromDate, DateTime toDate)
        {
            double timeDiff = toDate.Millisecond - fromDate.Millisecond;
            return Math.Ceiling(timeDiff / (1000 * 3600 * 24));
        }
        private string GetShortYear(DateTime date)
        {
            string fullYear = date.Year.ToString();
            if (fullYear.Length == 4)
                return fullYear.Substring(2);
            else
                return fullYear;
        }
        private string GetMonthNameShort(DateTime date)
        {
            string[] shortMonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            int monthIndex = date.Month;
            return shortMonthNames[monthIndex - 1];
        }
        private BIConfigurationTimeEntity GetUsedTimeEntityOrDefault(string timeEntityName = null)
        {
            List<BIConfiguration<BIConfigurationTimeEntity>> configurationTimeEntity = _configurations.GetTimeEntities();
            if (timeEntityName != null)
            {
                return configurationTimeEntity.Find(x => x.Name == timeEntityName).Configuration;
            }

            var defaultTimeEntity = configurationTimeEntity.Find(x => x.Configuration.IsDefault);
            if (defaultTimeEntity == null)
                throw new Exception("There is no default time entity.");
            return defaultTimeEntity.Configuration;
        }

        #endregion

    }
}
