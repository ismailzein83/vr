using System;
using System.Collections.Generic;
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
                    IdColumn = "z.CodeGroup",
                    NameColumn = "c.Name",
                    JoinStatements = new List<string>() { @" LEFT JOIN  OurZones ourz ON ts.OurZoneID = ourz.ZoneID LEFT JOIN CodeGroup c ON ourz.CodeGroup=c.Code" },
                    GroupByStatements = new List<string>() { " z.CodeGroup ,  c.Name" },
                    CTEStatement = " OurZones AS (SELECT ZoneID, Name, CodeGroup FROM Zone z WITH (NOLOCK) WHERE SupplierID = 'SYS') "
                });
        }

        private static void FillMeasureFieldsConfig()
        {
            s_AllMeasureFieldsConfig = new Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig>();

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.FirstCDRAttempt,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "Min(FirstCDRAttempt)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.ASR,
               new AnalyticMeasureFieldConfig
               {
                   GetFieldExpression = (query) =>
                       {
                           string s = "";
                           foreach(AnalyticDimension  dimension in query.DimensionFields)
                           {
                               if (dimension == AnalyticDimension.Supplier || dimension == AnalyticDimension.PortOut || dimension == AnalyticDimension.GateWayOut
                                   //|| (filter.SupplierIds != null && filter.SupplierIds.Count != 0)
                                   )
                                   s = "Attempts";
                               else
                                   s = "NumberOfCalls";
                           }
                           return String.Format("Case WHEN (Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/(Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END ", s);
                       }
               });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.ABR,
               new AnalyticMeasureFieldConfig
               {
                   GetFieldExpression = (query) =>
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
                       return String.Format("Case WHEN SUM(ts.{0})>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/Sum(ts.{0})) ELSE 0 END ", s);
                   }
               });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.NER,
               new AnalyticMeasureFieldConfig
               {
                   GetFieldExpression = (query) =>
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
                       return String.Format("Case WHEN (Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.DeliveredNumberOfCalls)*100.0/(Sum(ts.{0})-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END ", s);
                   }
               });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.Attempts,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "Sum(ts.Attempts)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.SuccessfulAttempts,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "Sum(ts.SuccessfulAttempts) / 60"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.FailedAttempts,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "Sum(ts.Attempts-ts.SuccessfulAttempts)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DeliveredAttempts,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "Sum(ts.DeliveredAttempts)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DurationsInSeconds,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "Sum(ts.DurationsInSeconds)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.PDDInSeconds,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "AVG(ts.PDDInSeconds)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.UtilizationInSeconds,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "AVG(ts.UtilizationInSeconds)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.NumberOfCalls,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "Sum(ts.NumberOfCalls)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.DeliveredNumberOfCalls,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "Sum(ts.DeliveredNumberOfCalls)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.CeiledDuration,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "Sum(ts.CeiledDuration)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.ACD,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "case when Sum(ts.SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(ts.DurationsInSeconds)/(60.0*Sum(ts.SuccessfulAttempts))) ELSE 0 end"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.LastCDRAttempt,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt))"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.MaxDurationInSeconds,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "CONVERT(DECIMAL(10,2),Max (ts.MaxDurationInSeconds)/60.0)"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.PGAD,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "CONVERT(DECIMAL(10,2),Avg(ts.PGAD))"
                });

            s_AllMeasureFieldsConfig.Add(AnalyticMeasureField.AveragePDD,
                new AnalyticMeasureFieldConfig
                {
                    GetFieldExpression = (query) => "CONVERT(DECIMAL(10,2),Avg(ts.PDDinSeconds))"
                });
        }
    }
}
