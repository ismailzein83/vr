using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data.SQL
{
    internal class GenericAnalyticConfigManager
    {
        static GenericAnalyticConfigManager()
        {
            FillDimensionsConfig();
            FillMeasureFieldsConfig();
        }

        static Dictionary<AnalyticDimension, AnalyticDimensionConfig> s_AllDimensionsConfig;
        static Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> s_AllMeasureFieldsConfig;

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

        private static void FillDimensionsConfig()
        {
            s_AllDimensionsConfig = new Dictionary<AnalyticDimension, AnalyticDimensionConfig>();

            s_AllDimensionsConfig.Add(AnalyticDimension.Zone,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.OurZoneID",
                    NameColumn = "z.Name",
                    JoinStatements = new List<string>() { " JOIN Zone z WITH (NOLOCK) ON z.ZoneID = ts.OurZoneID " },
                    GroupByStatements = new List<string>() { " ts.OurZoneID, z.Name " },
                    CTEStatement = null
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.Customer,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.CustomerID",
                    NameColumn = "case when cust.NameSuffix != '' THEN  custProf.Name + '(' + cust.NameSuffix + ')' else custProf.Name end",
                    JoinStatements = new List<string>() { @" JOIN CarrierAccount cust WITH (NOLOCK) ON cust.CarrierAccountID = ts.CustomerID
                                         JOIN CarrierProfile custProf on cust.ProfileID = custProf.ProfileID " },
                    GroupByStatements = new List<string>() { " ts.CustomerID, cust.NameSuffix, custProf.Name " },
                    CTEStatement = null
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.Supplier,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.SupplierID",
                    NameColumn = "case when supp.NameSuffix != '' THEN  suppProf.Name + '(' + supp.NameSuffix + ')' else suppProf.Name end",
                    JoinStatements = new List<string>() { @" JOIN CarrierAccount supp WITH (NOLOCK) ON supp.CarrierAccountID = ts.SupplierID
                                                     JOIN CarrierProfile suppProf on supp.ProfileID = suppProf.ProfileID " },
                    GroupByStatements = new List<string>() { " ts.SupplierID, supp.NameSuffix, suppProf.Name " },
                    CTEStatement = null
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.CodeGroup,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ourz.CodeGroup",
                    NameColumn = "c.Name",
                    JoinStatements = new List<string>() { @" LEFT JOIN  OurZones ourz ON ts.OurZoneID = ourz.ZoneID LEFT JOIN CodeGroup c ON ourz.CodeGroup=c.Code" },
                    GroupByStatements = new List<string>() { " ourz.CodeGroup,  c.Name" },
                    CTEStatement = " OurZones AS (SELECT ZoneID, Name, CodeGroup FROM Zone z WITH (NOLOCK) WHERE SupplierID = 'SYS') "
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.Switch,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.SwitchID",
                    NameColumn = "sw.Name",
                    JoinStatements = new List<string>() { @"JOIN Switch sw WITH (NOLOCK) ON sw.SwitchID = ts.SwitchID" },
                    GroupByStatements = new List<string>() { "ts.SwitchID, sw.Name" },
                    CTEStatement = null
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.GateWayIn,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ISNULL(cscIn.GateWayID,0)",
                    NameColumn = "ISNULL(cscIn.GateWayName,'N/A')",
                    JoinStatements = new List<string>() { @"Left JOIN SwitchConnectivity cscIn  ON (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' ) AND(ts.SwitchID = cscIn.SwitchID) AND ts.CustomerID =cscIn.CarrierAccount " },
                    GroupByStatements = new List<string>() { "cscIn.GateWayID, cscIn.GateWayName" },
                    CTEStatement = "SwitchConnectivity AS ( SELECT csc.CarrierAccountID AS  CarrierAccount ,csc.SwitchID AS SwitchID ,csc.Details AS Details ,csc.BeginEffectiveDate AS BeginEffectiveDate ,csc.EndEffectiveDate AS EndEffectiveDate ,csc.[Name] AS GateWayName ,csc.[ID] AS GateWayID FROM   CarrierSwitchConnectivity csc WITH(NOLOCK)  WHERE (csc.EndEffectiveDate IS null))"
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.GateWayOut,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ISNULL(cscOut.GateWayID,0)",
                    NameColumn = "ISNULL(cscOut.GateWayName,'N/A')",
                    JoinStatements = new List<string>() { @"Left JOIN SwitchConnectivityOut cscOut ON  (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%') AND (ts.SwitchID = cscOut.SwitchID)  AND ts.SupplierID  =cscOut.CarrierAccount" },
                    GroupByStatements = new List<string>() { "cscOut.GateWayID, cscOut.GateWayName" },
                    CTEStatement = "SwitchConnectivityOut AS ( SELECT csc.CarrierAccountID AS  CarrierAccount ,csc.SwitchID AS SwitchID ,csc.Details AS Details ,csc.BeginEffectiveDate AS BeginEffectiveDate ,csc.EndEffectiveDate AS EndEffectiveDate ,csc.[Name] AS GateWayName ,csc.[ID] AS GateWayID FROM   CarrierSwitchConnectivity csc WITH(NOLOCK)  WHERE (csc.EndEffectiveDate IS null))"
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.PortIn,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.Port_IN",
                    NameColumn = "ts.Port_IN",
                    JoinStatements = null,
                    GroupByStatements = new List<string>() { "ts.Port_IN" },
                    CTEStatement = null
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.PortOut,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.Port_OUT",
                    NameColumn = "ts.Port_OUT",
                    JoinStatements = null,
                    GroupByStatements = new List<string>() { "ts.Port_OUT" },
                    CTEStatement = null
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.CodeSales,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.OurCode",
                    NameColumn = "ts.OurCode",
                    JoinStatements = null,
                    GroupByStatements = new List<string>() { "ts.OurCode" },
                    CTEStatement = null
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.CodeBuy,
                new AnalyticDimensionConfig
                {
                    IdColumn = "ts.SupplierCode",
                    NameColumn = "ts.SupplierCode",
                    JoinStatements = null,
                    GroupByStatements = new List<string>() { "ts.SupplierCode" },
                    CTEStatement = null
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.Date,
                new AnalyticDimensionConfig
                {
                    IdColumn = "dateadd(dd,0, datediff(dd,0,LastCDRAttempt))",
                    NameColumn = "dateadd(dd,0, datediff(dd,0,LastCDRAttempt))",
                    JoinStatements = null,
                    GroupByStatements = new List<string>() { "dateadd(dd,0, datediff(dd,0,LastCDRAttempt))" },
                    CTEStatement = null
                });

            s_AllDimensionsConfig.Add(AnalyticDimension.Hour,
                new AnalyticDimensionConfig
                {
                    IdColumn = " datepart(hour,LastCDRAttempt)",
                    NameColumn = " datepart(hour,LastCDRAttempt)",
                    JoinStatements = null,
                    GroupByStatements = new List<string>() { " datepart(hour,LastCDRAttempt)" },
                    CTEStatement = null
                });
        }

        private static void FillMeasureFieldsConfig()
        {
            s_AllMeasureFieldsConfig = new Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig>();

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.FirstCDRAttempt,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.FirstCDRAttempt_Expression},
                    MappedSQLColumn = MeasureValueExpression.FirstCDRAttempt_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.FirstCDRAttempt_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.ASR,
                new AnalyticMeasureFieldConfig
                {
                     GetColumnsExpressions = new List<Func<AnalyticQuery,MeasureValueExpression>>{
                        (query) =>
                       {
                           string s = "";
                           foreach (AnalyticDimension dimension in query.DimensionFields)
                           {
                               if (dimension == AnalyticDimension.Supplier || dimension == AnalyticDimension.PortOut || dimension == AnalyticDimension.GateWayOut
                                   //|| (filter.SupplierIds != null && filter.SupplierIds.Count != 0)
                                   )
                                   s = "Attempts";
                               else
                                   s = "NumberOfCalls";
                           }
                           return new MeasureValueExpression
                           {
                               Expression = String.Format("Case WHEN (Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/(Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END ", s),
                               ColumnAlias = "Measure_ASR"
                           };
                       }
                   },
                     MappedSQLColumn = "Measure_ASR",
                     GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, "Measure_ASR")
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.ABR,
               new AnalyticMeasureFieldConfig
               {
                   GetColumnsExpressions = new List<Func<AnalyticQuery,MeasureValueExpression>>{
                        (query) =>
                       {
                           string s = "";
                           foreach (AnalyticDimension dimension in query.DimensionFields)
                           {
                               if (dimension == AnalyticDimension.Supplier || dimension == AnalyticDimension.PortOut || dimension == AnalyticDimension.GateWayOut
                                   //|| (filter.SupplierIds != null && filter.SupplierIds.Count != 0)
                                   )
                                   s = "Attempts";
                               else
                                   s = "NumberOfCalls";
                           }
                           return new MeasureValueExpression
                           {
                               Expression = String.Format("Case WHEN SUM(ts.{0})>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/Sum(ts.{0})) ELSE 0 END ", s),
                               ColumnAlias = "Measure_ABR"
                           };
                       }
                   },
                   MappedSQLColumn = "Measure_ABR",
                   GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, "Measure_ABR")

               });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.NER,
               new AnalyticMeasureFieldConfig
               {
                    GetColumnsExpressions = new List<Func<AnalyticQuery,MeasureValueExpression>>{
                        (query) =>
                       {
                           string s = "";
                           foreach (AnalyticDimension dimension in query.DimensionFields)
                           {
                               if (dimension == AnalyticDimension.Supplier || dimension == AnalyticDimension.PortOut || dimension == AnalyticDimension.GateWayOut
                                   //|| (filter.SupplierIds != null && filter.SupplierIds.Count != 0)
                                   )
                                   s = "Attempts";
                               else
                                   s = "NumberOfCalls";
                           }
                           return new MeasureValueExpression
                           {
                               Expression = String.Format("Case WHEN (Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.DeliveredNumberOfCalls)*100.0/(Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END ", s),
                               ColumnAlias = "Measure_NER"
                           };
                       }
                   },
                    MappedSQLColumn = "Measure_NER",
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, "Measure_NER")
               });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.Attempts,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.Attempts_Expression},
                    MappedSQLColumn = MeasureValueExpression.Attempts_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.Attempts_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.SuccessfulAttempts,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.SuccessfulAttempts_Expression},
                    MappedSQLColumn = MeasureValueExpression.SuccessfulAttempts_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.SuccessfulAttempts_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.FailedAttempts,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.FailedAttempts_Expression },
                    MappedSQLColumn = MeasureValueExpression.FailedAttempts_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.FailedAttempts_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DeliveredAttempts,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.DeliveredAttempts_Expression },
                    MappedSQLColumn = MeasureValueExpression.DeliveredAttempts_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.DeliveredAttempts_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DurationsInSeconds,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.DurationsInSeconds_Expression },
                    MappedSQLColumn =  MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.PDDInSeconds,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.PDDInSeconds_Expression },
                    MappedSQLColumn = MeasureValueExpression.PDDInSeconds_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.PDDInSeconds_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.UtilizationInSeconds,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.UtilizationInSeconds_Expression },
                    MappedSQLColumn = MeasureValueExpression.UtilizationInSeconds_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.UtilizationInSeconds_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.NumberOfCalls,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.NumberOfCalls_Expression },
                    MappedSQLColumn = MeasureValueExpression.NumberOfCalls_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.NumberOfCalls_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DeliveredNumberOfCalls,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.DeliveredNumberOfCalls_Expression },
                    MappedSQLColumn = MeasureValueExpression.DeliveredNumberOfCalls_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.DeliveredNumberOfCalls_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.CeiledDuration,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.CeiledDuration_Expression },
                    MappedSQLColumn = MeasureValueExpression.CeiledDuration_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.CeiledDuration_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.ACD,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> {
                        (query) => MeasureValueExpression.SuccessfulAttempts_Expression,
                        (query) => MeasureValueExpression.DurationsInSeconds_Expression
                    },
                    GetMeasureValue = (reader, record) =>
                    {
                        var successfulAttempts = GetReaderValue<int>(reader, MeasureValueExpression.SuccessfulAttempts_Expression.ColumnAlias);
                        var durationInMinutes = GetReaderValue<Decimal>(reader, MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias) / 60;
                        return successfulAttempts > 0 ? (durationInMinutes * successfulAttempts) : 0;
                    }
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.LastCDRAttempt,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.LastCDRAttempt_Expression },
                    MappedSQLColumn = MeasureValueExpression.LastCDRAttempt_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.LastCDRAttempt_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.MaxDurationInSeconds,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.MaxDurationInSeconds_Expression },
                    MappedSQLColumn = MeasureValueExpression.MaxDurationInSeconds_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.MaxDurationInSeconds_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.PGAD,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.PGAD_Expression },
                    MappedSQLColumn = MeasureValueExpression.PGAD_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.PGAD_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.AveragePDD,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> { (query) => MeasureValueExpression.AveragePDD_Expression },
                    MappedSQLColumn = MeasureValueExpression.AveragePDD_Expression.ColumnAlias,
                    GetMeasureValue = (reader, record) => GetReaderValue<Object>(reader, MeasureValueExpression.AveragePDD_Expression.ColumnAlias)
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.GreenArea,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery,MeasureValueExpression>> {
                        (query) => MeasureValueExpression.DurationsInSeconds_Expression,
                        (query) => MeasureValueExpression.UtilizationInSeconds_Expression,
                        (query) => MeasureValueExpression.NominalCapacityInE1s_Expression
                    },
                    GetMeasureValue = (reader, record) => 
                        {
                            var nominalCapacity = GetReaderValue<int>(reader, MeasureValueExpression.NominalCapacityInE1s_Expression.ColumnAlias);
                            var utilizationInMinutes = GetReaderValue<Decimal>(reader, MeasureValueExpression.UtilizationInSeconds_Expression.ColumnAlias) / 60;
                            var durationInMinutes = GetReaderValue<Decimal>(reader, MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias) / 60;
                            return nominalCapacity > 0 ? ((durationInMinutes * 100) / nominalCapacity) : 0;
                        }
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.GrayArea,
                new AnalyticMeasureFieldConfig
                {
                    GetColumnsExpressions = new List<Func<AnalyticQuery, MeasureValueExpression>> {
                        (query) => MeasureValueExpression.DurationsInSeconds_Expression,
                        (query) => MeasureValueExpression.UtilizationInSeconds_Expression,
                        (query) => MeasureValueExpression.NominalCapacityInE1s_Expression
                    },
                    GetMeasureValue = (reader, record) =>
                    {
                        var nominalCapacity = GetReaderValue<int>(reader, MeasureValueExpression.NominalCapacityInE1s_Expression.ColumnAlias);
                        var utilizationInMinutes = GetReaderValue<Decimal>(reader, MeasureValueExpression.UtilizationInSeconds_Expression.ColumnAlias) / 60;
                        var durationInMinutes = GetReaderValue<Decimal>(reader, MeasureValueExpression.DurationsInSeconds_Expression.ColumnAlias) / 60;
                        return nominalCapacity > 0 ? ((utilizationInMinutes - durationInMinutes * 100) / nominalCapacity) : 0;
                    }
                });
        }

        static T GetReaderValue<T>(IDataReader reader, string fieldName)
        {
            return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
        }
    }
}
