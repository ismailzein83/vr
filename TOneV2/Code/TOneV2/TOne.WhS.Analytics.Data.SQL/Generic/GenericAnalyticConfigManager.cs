using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Data.SQL
{
    internal class GenericAnalyticConfigManager
    {
      
        #region ctor/Local Variables
        static GenericAnalyticConfigManager()
        {
            FillDimensionsConfig();
            FillMeasureFieldsConfig();
        }
        static Dictionary<AnalyticDimension, AnalyticDimensionConfig> s_AllDimensionsConfig;
        static Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> s_AllMeasureFieldsConfig;

        #endregion

        #region Public Methods
        public Dictionary<AnalyticDimension, AnalyticDimensionConfig> GetGroupFieldsConfig(IEnumerable<AnalyticDimension> fields)
        {
            Dictionary<AnalyticDimension, AnalyticDimensionConfig> result = new Dictionary<AnalyticDimension, AnalyticDimensionConfig>();
            foreach (AnalyticDimension itm in fields)
            {
                var item = s_AllDimensionsConfig[itm];
                if (!result.ContainsKey(itm))
                    result.Add(itm, item);
            }
            return result;
        }
        public Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> GetMeasureFieldsConfig(IEnumerable<AnalyticMeasureField> fields)
        {
            Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> result = new Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig>();
            foreach (var itm in s_AllMeasureFieldsConfig)
            {
                if (fields.Contains(itm.Key))
                    result.Add(itm.Key, itm.Value);
            }
            return result;
        }
        #endregion

        #region Private Methods
        private static void FillDimensionsConfig()
        {
            s_AllDimensionsConfig = new Dictionary<AnalyticDimension, AnalyticDimensionConfig>();

            #region ZonePart
            s_AllDimensionsConfig.Add(AnalyticDimension.Zone,
               new AnalyticDimensionConfig
               {
                   IdColumn = "ISNULL(ts.SaleZoneID,'N/A')",
                   NameColumn = "z.Name",
                   JoinStatements = new List<string>() { " JOIN TOneWhS_BE.SaleZone z WITH (NOLOCK) ON z.ID = ts.SaleZoneID " },
                   GroupByStatements = new List<string>() { " ts.SaleZoneID, z.Name " },
                   ExpressionSummary = AnalyticSummary.Sum.ToString("G")
               });
            #endregion

            #region SupplierZonePart
            s_AllDimensionsConfig.Add(AnalyticDimension.SupplierZone,
               new AnalyticDimensionConfig
               {
                   IdColumn = "ISNULL(ts.SupplierZoneID,'N/A')",
                   NameColumn = "z.Name",
                   JoinStatements = new List<string>() { " JOIN Zone z WITH (NOLOCK) ON z.ZoneID = ts.SupplierZoneID " },
                   GroupByStatements = new List<string>() { " ts.SupplierZoneID, z.Name " },
                   ExpressionSummary = AnalyticSummary.Sum.ToString("G")
               });
            #endregion

            #region CustomerPart
            s_AllDimensionsConfig.Add(AnalyticDimension.Customer,
             new AnalyticDimensionConfig
             {
                 IdColumn = "ISNULL(ts.CustomerID,0)",
                 NameColumn = "custProf.Name",//"ISNULL(case when cust.NameSuffix != '' THEN  custProf.Name + '(' + cust.NameSuffix + ')' else custProf.Name end,'N/A')",
                 JoinStatements = new List<string>() { @"LEFT JOIN TOneWhS_BE.CarrierAccount cust WITH (NOLOCK) ON cust.ID = ts.CustomerID
                                                            LEFT JOIN TOneWhS_BE.CarrierProfile custProf on cust.CarrierProfileID = custProf.ID " },
                 GroupByStatements = new List<string>() { " ts.CustomerID, custProf.Name " },//cust.NameSuffix, custProf.Name " },
                 ExpressionSummary = AnalyticSummary.Sum.ToString("G")
             });
            #endregion

            #region SupplierPart
            s_AllDimensionsConfig.Add(AnalyticDimension.Supplier,
               new AnalyticDimensionConfig
               {
                   IdColumn = "ISNULL(ts.SupplierID,0)",
                   NameColumn = "suppProf.Name",//"ISNULL(case when supp.NameSuffix != '' THEN  suppProf.Name + '(' + supp.NameSuffix + ')' else suppProf.Name end,'N/A')",
                   JoinStatements = new List<string>() { @"LEFT JOIN TOneWhS_BE.CarrierAccount supp WITH (NOLOCK) ON supp.ID = ts.SupplierID
                                                            LEFT JOIN TOneWhS_BE.CarrierProfile suppProf on supp.CarrierProfileID = suppProf.ID " },
                   GroupByStatements = new List<string>() { " ts.SupplierID,suppProf.Name " },// supp.NameSuffix, suppProf.Name " },
                   ExpressionSummary = AnalyticSummary.Sum.ToString("G")
               });
            #endregion

            #region CountryPart

            s_AllDimensionsConfig.Add(AnalyticDimension.Country,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ourz.CountryID",
                    NameColumn = "c.Name",
                    JoinStatements = new List<string>() { @" LEFT JOIN  TOneWhS_BE.SaleZone ourz ON ts.SaleZoneID = ourz.ID LEFT JOIN [Common].Country c ON ourz.CountryID = c.ID" },
                    GroupByStatements = new List<string>() { " ourz.CountryID,  c.Name" },
                    ExpressionSummary = AnalyticSummary.Sum.ToString("G")
                });
            #endregion

            #region SwitchPart
            s_AllDimensionsConfig.Add(AnalyticDimension.Switch,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.SwitchID",
                    NameColumn = "sw.Name",
                    JoinStatements = new List<string>() { @"JOIN [TOneWhS_BE].[Switch] sw WITH (NOLOCK) ON sw.ID = ts.SwitchID" },
                    GroupByStatements = new List<string>() { "ts.SwitchID, sw.Name" },
                    ExpressionSummary = AnalyticSummary.Sum.ToString("G")
                });
            #endregion

            #region PortInPart
            s_AllDimensionsConfig.Add(AnalyticDimension.PortIn,
              new AnalyticDimensionConfig
              {
                  IdColumn = "ts.PortIn",
                  NameColumn = "ts.PortIn",
                  JoinStatements = null,
                  GroupByStatements = new List<string>() { "ts.PortIn" },
                  ExpressionSummary = AnalyticSummary.Sum.ToString("G")
              });
            #endregion

            #region PortOutPart
            s_AllDimensionsConfig.Add(AnalyticDimension.PortOut,
            new AnalyticDimensionConfig
            {
                IdColumn = "ts.PortOut",
                NameColumn = "ts.PortOut",
                JoinStatements = null,
                GroupByStatements = new List<string>() { "ts.PortOut" },
                ExpressionSummary = AnalyticSummary.Sum.ToString("G")
            });
            #endregion

            #region CodeSalesPart
            s_AllDimensionsConfig.Add(AnalyticDimension.CodeSales,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.OurCode",
                    NameColumn = "ts.OurCode",
                    JoinStatements = null,
                    GroupByStatements = new List<string>() { "ts.OurCode" },
                    ExpressionSummary = AnalyticSummary.Sum.ToString("G")
                });
            #endregion

            #region CodeBuyPart
            s_AllDimensionsConfig.Add(AnalyticDimension.CodeBuy,
              new AnalyticDimensionConfig
              {
                  IdColumn = "ISNULL(ts.SupplierCode,'N/A')",
                  NameColumn = "ISNULL(ts.SupplierCode,'N/A')",
                  JoinStatements = null,
                  GroupByStatements = new List<string>() { "ts.SupplierCode" },
                  ExpressionSummary = AnalyticSummary.Sum.ToString("G")
              });
            #endregion

            #region DatePart
            s_AllDimensionsConfig.Add(AnalyticDimension.Date,
               new AnalyticDimensionConfig
               {
                   IdColumn = "CONVERT(VARCHAR(10),dateadd(dd,0, datediff(dd,0,LastCDRAttempt)),101)",
                   NameColumn = "CONVERT(VARCHAR(10),dateadd(dd,0, datediff(dd,0,LastCDRAttempt)),101)",
                   JoinStatements = null,
                   GroupByStatements = new List<string>() { "dateadd(dd,0, datediff(dd,0,LastCDRAttempt))" },
                   ExpressionSummary = AnalyticSummary.Max.ToString("G")
               });


            #endregion

            #region HourPart
            s_AllDimensionsConfig.Add(AnalyticDimension.Hour,
               new AnalyticDimensionConfig
               {
                   IdColumn = " datepart(hour,LastCDRAttempt)",
                   NameColumn = " datepart(hour,LastCDRAttempt)",
                   JoinStatements = null,
                   GroupByStatements = new List<string>() { " datepart(hour,LastCDRAttempt)" },
                   ExpressionSummary = AnalyticSummary.Max.ToString("G")
               });
            #endregion

            #region DayPart
            s_AllDimensionsConfig.Add(AnalyticDimension.Day,
             new AnalyticDimensionConfig
             {
                 IdColumn = "CONVERT(VARCHAR(10), FirstCDRAttempt,121)",
                 NameColumn = "CONVERT(VARCHAR(10),FirstCDRAttempt,121)",
                 JoinStatements = null,
                 GroupByStatements = new List<string>() { "CONVERT(VARCHAR(10), FirstCDRAttempt,121)" },
                 ExpressionSummary = AnalyticSummary.Max.ToString("G")
             });

            #endregion

            #region WeekPart
            s_AllDimensionsConfig.Add(AnalyticDimension.Week,
               new AnalyticDimensionConfig
               {
                   IdColumn = "cast(datepart(yyyy, FirstCDRAttempt) AS varchar) + ', Week ' + cast(datepart(wk, FirstCDRAttempt) AS varchar)",
                   NameColumn = "cast(datepart(yyyy, FirstCDRAttempt) AS varchar) +  ', Week '  + cast(datepart(wk, FirstCDRAttempt) AS varchar)",
                   JoinStatements = null,
                   GroupByStatements = new List<string>() { "cast(datepart(yyyy, FirstCDRAttempt) AS varchar) +  ', Week '  + cast(datepart(wk, FirstCDRAttempt) AS varchar)" },
                   ExpressionSummary = AnalyticSummary.Max.ToString("G")
               });

            #endregion

            #region MonthPart
            s_AllDimensionsConfig.Add(AnalyticDimension.Month,
               new AnalyticDimensionConfig
               {
                   IdColumn = "CONVERT(VARCHAR(7), FirstCDRAttempt,121)",
                   NameColumn = "CONVERT(VARCHAR(7),FirstCDRAttempt,121)",
                   JoinStatements = null,
                   GroupByStatements = new List<string>() { "CONVERT(VARCHAR(7), FirstCDRAttempt,121)" },
                   ExpressionSummary = AnalyticSummary.Max.ToString("G")
               });
            #endregion

            //s_AllDimensionsConfig.Add(AnalyticDimension.GateWayIn,
            //    new AnalyticDimensionConfig
            //    {
            //        IdColumn = "ISNULL(cscIn.GateWayID,0)",
            //        NameColumn = "ISNULL(cscIn.GateWayName,'N/A')",
            //        JoinStatements = new List<string>() { @"LEFT JOIN SwitchConnectivity cscIn  ON (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' ) AND(ts.SwitchID = cscIn.SwitchID) AND ts.CustomerID = cscIn.CarrierAccount " },
            //        GroupByStatements = new List<string>() { "cscIn.GateWayID, cscIn.GateWayName" },
            //        ExpressionSummary = AnalyticSummary.Sum.ToString("G")
            //    });

            //s_AllDimensionsConfig.Add(AnalyticDimension.GateWayOut,
            //    new AnalyticDimensionConfig
            //    {
            //        IdColumn = "ISNULL(cscOut.GateWayID,0)",
            //        NameColumn = "ISNULL(cscOut.GateWayName,'N/A')",
            //        JoinStatements = new List<string>() { @"LEFT JOIN SwitchConnectivity cscOut ON  (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%') AND (ts.SwitchID = cscOut.SwitchID)  AND ts.SupplierID  = cscOut.CarrierAccount" },
            //        GroupByStatements = new List<string>() { "cscOut.GateWayID, cscOut.GateWayName" },
            //        ExpressionSummary = AnalyticSummary.Sum.ToString("G")
            //    });

        }
        private static void FillMeasureFieldsConfig()
        {
            s_AllMeasureFieldsConfig = new Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig>();

            #region FirstCDRAttemptPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.FirstCDRAttempt,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.FirstCDRAttempt_Expression },
                   MappedSQLColumn = MeasureValueExpression.FirstCDRAttempt_Expression.ColumnAlias,
                   GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.FirstCDRAttempt_Expression.ColumnAlias)
               });
            #endregion

            #region ABRPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.ABR,
              new AnalyticMeasureFieldConfig
              {
                  GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>>{
                        (query) =>
                       {
                           string s = "NumberOfCalls";
                           bool filterSupplier = false;
                           foreach (DimensionFilter dimensionFilter in query.Filters)
                           {
                               if (dimensionFilter.Dimension == AnalyticDimension.Supplier)
                                   filterSupplier = true;
                           }
                           foreach (AnalyticDimension dimension in query.DimensionFields)
                           {
                               if (dimension == AnalyticDimension.Supplier || dimension == AnalyticDimension.PortOut || dimension == AnalyticDimension.GateWayOut || filterSupplier)
                                   s = "Attempts";
                               else
                                   s = "NumberOfCalls";
                           }
                           return new MeasureValueExpression
                           {
                               Expression = String.Format("Case WHEN SUM(ts.{0})>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/Sum(ts.{0})) ELSE 0 END ", s),
                               ColumnAlias = "Measure_ABR",
                               ExpressionSummary = AnalyticSummary.Sum.ToString("G")
                           };
                       }
                   },
                  MappedSQLColumn = "Measure_ABR",
                  GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, "Measure_ABR")

              });

            #endregion

            #region ASRPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.ASR,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>>{
                        (query) =>
                       {
                           string s = "NumberOfCalls";
                           bool filterSupplier = false;
                           if (query.Filters != null)
                           {
                               foreach (DimensionFilter dimensionFilter in query.Filters)
                               {
                                   if (dimensionFilter.Dimension == AnalyticDimension.Supplier)
                                       filterSupplier = true;
                               }
                           }
                           
                           foreach (AnalyticDimension dimension in query.DimensionFields)
                           {
                               if (dimension == AnalyticDimension.Supplier || dimension == AnalyticDimension.PortOut || dimension == AnalyticDimension.GateWayOut || filterSupplier)
                                   s = "Attempts";
                               else
                                   s = "NumberOfCalls";
                           }
                           return new MeasureValueExpression
                           {
                               Expression = String.Format("Case WHEN (Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/(Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END ", s),
                               ColumnAlias = "Measure_ASR",
                               ExpressionSummary = AnalyticSummary.Sum.ToString("G")
                           };
                       }
                   },
                   MappedSQLColumn = "Measure_ASR",
                   GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, "Measure_ASR")
               });
            #endregion

            #region NERPart

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.NER,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>>{
                        (query) =>
                       {
                           string s = "NumberOfCalls";                            
                           bool filterSupplier = false;
                           if (query.Filters != null)
                           {
                               foreach (DimensionFilter dimensionFilter in query.Filters)
                               {
                                   if (dimensionFilter.Dimension == AnalyticDimension.Supplier)
                                       filterSupplier = true;
                               }
                           }
                           
                           foreach (AnalyticDimension dimension in query.DimensionFields)
                           {
                               if (dimension == AnalyticDimension.Supplier || dimension == AnalyticDimension.PortOut || dimension == AnalyticDimension.GateWayOut || filterSupplier)
                                   s = "Attempts";
                               else
                                   s = "NumberOfCalls";
                           }
                           return new MeasureValueExpression
                           {
                               Expression = String.Format("Case WHEN (Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.DeliveredNumberOfCalls)*100.0/(Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END ", s),
                               ColumnAlias = "Measure_NER",
                               ExpressionSummary = AnalyticSummary.Sum.ToString("G")
                           };
                       }
                   },
                   MappedSQLColumn = "Measure_NER",
                   GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, "Measure_NER")
               });
            #endregion

            #region AttemptsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.Attempts,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.Attempts_Expression },
                    MappedSQLColumn = MeasureValueExpression.Attempts_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.Attempts_Expression.ColumnAlias)
                });
            #endregion

            #region SuccessfulAttemptsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.SuccessfulAttempts,
              new AnalyticMeasureFieldConfig
              {
                  GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.SuccessfulAttempts_Expression },
                  MappedSQLColumn = MeasureValueExpression.SuccessfulAttempts_Expression.ColumnAlias,
                  GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.SuccessfulAttempts_Expression.ColumnAlias)
              });
            #endregion

            #region FailedAttemptsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.FailedAttempts,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.FailedAttempts_Expression },
                   MappedSQLColumn = MeasureValueExpression.FailedAttempts_Expression.ColumnAlias,
                   GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.FailedAttempts_Expression.ColumnAlias)
               });
            #endregion

            #region DeliveredAttemptsPart

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DeliveredAttempts,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.DeliveredAttempts_Expression },
                    MappedSQLColumn = MeasureValueExpression.DeliveredAttempts_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<int>(reader, MeasureValueExpression.DeliveredAttempts_Expression.ColumnAlias)
                });
            #endregion

            #region DurationsInSecondsPart

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DurationsInSeconds,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.DurationsInSeconds_Expression },
                    MappedSQLColumn = MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<int>(reader, MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias)
                });
            #endregion

            #region PDDInSecondsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.PDDInSeconds,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.PDDInSeconds_Expression },
                   MappedSQLColumn = MeasureValueExpression.PDDInSeconds_Expression.ColumnAlias,
                   GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.PDDInSeconds_Expression.ColumnAlias)
               });
            #endregion

            #region NumberOfCallsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.NumberOfCalls,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.NumberOfCalls_Expression },
                   MappedSQLColumn = MeasureValueExpression.NumberOfCalls_Expression.ColumnAlias,
                   GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.NumberOfCalls_Expression.ColumnAlias)
               });
            #endregion

            #region DeliveredNumberOfCallsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DeliveredNumberOfCalls,
            new AnalyticMeasureFieldConfig
            {
                GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.DeliveredNumberOfCalls_Expression },
                MappedSQLColumn = MeasureValueExpression.DeliveredNumberOfCalls_Expression.ColumnAlias,
                GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.DeliveredNumberOfCalls_Expression.ColumnAlias)
            });
            #endregion

            #region CeiledDurationPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.CeiledDuration,
              new AnalyticMeasureFieldConfig
              {
                  GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.CeiledDuration_Expression },
                  MappedSQLColumn = MeasureValueExpression.CeiledDuration_Expression.ColumnAlias,
                  GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.CeiledDuration_Expression.ColumnAlias)
              });
            #endregion

            #region ACDPart

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.ACD,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> {
                        (query) => MeasureValueExpression.SuccessfulAttempts_Expression,
                        (query) => MeasureValueExpression.DurationsInSeconds_Expression
                    },
                    MappedSQLColumn = "Measure_ACD",
                    GetMeasureValue = (reader, record) =>
                    {
                        var successfulAttempts = GetReaderValue<int>(reader, MeasureValueExpression.SuccessfulAttempts_Expression.ColumnAlias);
                        var durationInMinutes = GetReaderValue<int>(reader, MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias) / 60;
                        return successfulAttempts > 0 ? (durationInMinutes * successfulAttempts) : 0;
                    }
                });
            #endregion

            #region DeliveredASRPart

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DeliveredASR,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> {
                                    (query) => MeasureValueExpression.Attempts_Expression,
                                    (query) => MeasureValueExpression.DeliveredAttempts_Expression
                                },
                    MappedSQLColumn = "Measure_DeliveredASR",
                    GetMeasureValue = (reader, record) =>
                    {
                        var attempts = GetReaderValue<int>(reader, MeasureValueExpression.Attempts_Expression.ColumnAlias);
                        var deliveredAttempts = GetReaderValue<int>(reader, MeasureValueExpression.DeliveredAttempts_Expression.ColumnAlias) * 100;
                        return deliveredAttempts / attempts;
                    }
                });
            #endregion

            #region LastCDRAttemptPart

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.LastCDRAttempt,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.LastCDRAttempt_Expression },
                    MappedSQLColumn = MeasureValueExpression.LastCDRAttempt_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.LastCDRAttempt_Expression.ColumnAlias)
                });

            #endregion

            #region MaxDurationInSecondsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.MaxDurationInSeconds,
              new AnalyticMeasureFieldConfig
              {
                  GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.MaxDurationInSeconds_Expression },
                  MappedSQLColumn = MeasureValueExpression.MaxDurationInSeconds_Expression.ColumnAlias,
                  GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.MaxDurationInSeconds_Expression.ColumnAlias)
              });
            #endregion

            #region PGADPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.PGAD,
             new AnalyticMeasureFieldConfig
             {
                 GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.PGAD_Expression },
                 MappedSQLColumn = MeasureValueExpression.PGAD_Expression.ColumnAlias,
                 GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.PGAD_Expression.ColumnAlias)
             });
            #endregion

            #region AveragePDDPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.AveragePDD,
             new AnalyticMeasureFieldConfig
             {
                 GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.AveragePDD_Expression },
                 MappedSQLColumn = MeasureValueExpression.AveragePDD_Expression.ColumnAlias,
                 GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.AveragePDD_Expression.ColumnAlias)
             });
            #endregion

            #region GreenAreaPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.GreenArea,
           new AnalyticMeasureFieldConfig
           {
               GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> {
                        (query) => MeasureValueExpression.DurationsInSeconds_Expression,
                        (query) => MeasureValueExpression.UtilizationInSeconds_Expression,
                        (query) => MeasureValueExpression.NominalCapacityInE1s_Expression
                    },
               MappedSQLColumn = "Measure_GreenArea",
               GetMeasureValue = (reader, record) =>
               {
                   var nominalCapacity = GetReaderValue<int>(reader, MeasureValueExpression.NominalCapacityInE1s_Expression.ColumnAlias);
                   var durationInMinutes = GetReaderValue<int>(reader, MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias) / 60;
                   return nominalCapacity > 0 ? ((durationInMinutes * 100) / nominalCapacity) : 0;
               }
           });
            #endregion

            #region GrayAreaPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.GrayArea,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> {
                        (query) => MeasureValueExpression.DurationsInSeconds_Expression,
                        (query) => MeasureValueExpression.UtilizationInSeconds_Expression,
                        (query) => MeasureValueExpression.NominalCapacityInE1s_Expression
                    },
                   MappedSQLColumn = "Measure_GrayArea",
                   GetMeasureValue = (reader, record) =>
                   {
                       var nominalCapacity = GetReaderValue<int>(reader, MeasureValueExpression.NominalCapacityInE1s_Expression.ColumnAlias);
                       var utilizationInMinutes = GetReaderValue<Decimal>(reader, MeasureValueExpression.UtilizationInSeconds_Expression.ColumnAlias) / 60;
                       var durationInMinutes = GetReaderValue<int>(reader, MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias) / 60;
                       return nominalCapacity > 0 ? ((utilizationInMinutes - durationInMinutes * 100) / nominalCapacity) : 0;
                   }
               });
            #endregion

            #region BillingNumberOfCallsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.BillingNumberOfCalls,
             new AnalyticMeasureFieldConfig
             {
                 GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.BillingNumberOfCalls_Expression },
                 MappedSQLColumn = MeasureValueExpression.BillingNumberOfCalls_Expression.ColumnAlias,
                 GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.BillingNumberOfCalls_Expression.ColumnAlias)
             });
            #endregion

            #region CostNetsPart

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.CostNets,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.CostNets_Expression },
                    MappedSQLColumn = MeasureValueExpression.CostNets_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<decimal>(reader, MeasureValueExpression.CostNets_Expression.ColumnAlias)
                });

            #endregion

            #region SaleNetsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.SaleNets,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.SaleNets_Expression },
                   MappedSQLColumn = MeasureValueExpression.SaleNets_Expression.ColumnAlias,
                   GetMeasureValue = (reader, record) => GetReaderValue<decimal>(reader, MeasureValueExpression.SaleNets_Expression.ColumnAlias)
               });
            #endregion

            #region SaleRatePart

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.SaleRate,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.SaleRate_Expression },
                    MappedSQLColumn = MeasureValueExpression.SaleRate_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.SaleRate_Expression.ColumnAlias)
                });
            #endregion

            #region CostRatePart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.CostRate,
             new AnalyticMeasureFieldConfig
             {
                 GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.CostRate_Expression },
                 MappedSQLColumn = MeasureValueExpression.CostRate_Expression.ColumnAlias,
                 GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.CostRate_Expression.ColumnAlias)
             });
            #endregion

            #region PricedDurationPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.PricedDuration,
             new AnalyticMeasureFieldConfig
             {
                 GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.PricedDuration_Expression },
                 MappedSQLColumn = MeasureValueExpression.PricedDuration_Expression.ColumnAlias,
                 GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.PricedDuration_Expression.ColumnAlias)
             });

            #endregion

            #region ProfitPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.Profit,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> {
                                    (query) => MeasureValueExpression.CostNets_Expression,
                                    (query) => MeasureValueExpression.SaleNets_Expression
                                },
                   MappedSQLColumn = "Measure_Profit",
                   GetMeasureValue = (reader, record) =>
                   {
                       var costNets = GetReaderValue<decimal>(reader, MeasureValueExpression.CostNets_Expression.ColumnAlias);
                       var saleNets = GetReaderValue<decimal>(reader, MeasureValueExpression.SaleNets_Expression.ColumnAlias);
                       return saleNets - costNets;
                   }
               });
            #endregion

            #region CapacityUsageDetailsPart
            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.CapacityUsageDetails,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> {
                                    (query) => MeasureValueExpression.DurationsInSeconds_Expression,
                                    (query) => MeasureValueExpression.UtilizationInSeconds_Expression,
                                    (query) => MeasureValueExpression.NominalCapacityInE1s_Expression
                                            },
                   MappedSQLColumn = "Measure_CapacityUsageDetails",
                   GetMeasureValue = (reader, record) =>
                   {
                       var nominalCapacity = GetReaderValue<int>(reader, MeasureValueExpression.NominalCapacityInE1s_Expression.ColumnAlias);
                       var utilizationInMinutes = GetReaderValue<Decimal>(reader, MeasureValueExpression.UtilizationInSeconds_Expression.ColumnAlias) / 60;
                       var durationInMinutes = GetReaderValue<int>(reader, MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias) / 60;
                       return (nominalCapacity > 0 ? ((utilizationInMinutes - durationInMinutes * 100) / nominalCapacity) : 0).ToString() + "|"
                           + (nominalCapacity > 0 ? ((durationInMinutes * 100) / nominalCapacity) : 0).ToString();
                   }
               });
            #endregion

            #region UtilizationInSecondsPart
            //s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.UtilizationInSeconds,
            //    new AnalyticMeasureFieldConfig
            //    {
            //        GetColumnsExpressions = new List<Func<GenericAnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.UtilizationInSeconds_Expression },
            //        MappedSQLColumn = MeasureValueExpression.UtilizationInSeconds_Expression.ColumnAlias,
            //        GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.UtilizationInSeconds_Expression.ColumnAlias)
            //    });
            #endregion

        }
        private static T GetReaderValue<T>(IDataReader reader, string fieldName)
        {
            return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
        }
        #endregion
       
       
    }
}
