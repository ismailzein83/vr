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
        public GenericAnalyticConfigManager()
        {
            FillGroupFieldsConfig();
            //FillMeasureFieldsConfig();
        }

        static Dictionary<AnalyticGroupField, AnalyticGroupFieldConfig> s_AllGroupFieldsConfig;
        static Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> s_AllMeasureFieldsConfig;
        public Dictionary<AnalyticGroupField, AnalyticGroupFieldConfig> GetGroupFieldsConfig(IEnumerable<AnalyticGroupField> fields)
        {
            Dictionary<AnalyticGroupField, AnalyticGroupFieldConfig> result = new Dictionary<AnalyticGroupField, AnalyticGroupFieldConfig>();
            foreach (var itm in s_AllGroupFieldsConfig)
            {
                if (fields.Contains(itm.Key))
                    result.Add(itm.Key, itm.Value);
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

        private static void FillGroupFieldsConfig()
        {
            s_AllGroupFieldsConfig = new Dictionary<AnalyticGroupField, AnalyticGroupFieldConfig>();

            s_AllGroupFieldsConfig.Add(AnalyticGroupField.OwnZone,
                new AnalyticGroupFieldConfig
                {
                    IdColumn = "ts.OurZoneID",
                    NameColumn = "z.Name",
                    JoinStatements = new List<string>() { "JOIN Zone z WITH (NOLOCK) ON z.ZoneID = ts.OurZoneID" }
                });

            s_AllGroupFieldsConfig.Add(AnalyticGroupField.Customer,
                new AnalyticGroupFieldConfig
                {
                    IdColumn = "ts.CustomerID",
                    NameColumn = "case when cust.NameSuffix != '' THEN  custProf.Name + '(' + cust.NameSuffix + ')' else custProf.Name end",
                    JoinStatements = new List<string>() { @"JOIN CarrierAccount cust WITH (NOLOCK) ON cust.CarrierAccountID = ts.CustomerID
                                         JOIN CarrierProfile custProf on cust.ProfileID = custProf.ProfileID" }
                });

            s_AllGroupFieldsConfig.Add(AnalyticGroupField.Supplier,
                new AnalyticGroupFieldConfig
                {
                    IdColumn = "ts.SupplierID",
                    NameColumn = "case when cust.NameSuffix != '' THEN  custProf.Name + '(' + cust.NameSuffix + ')' else custProf.Name end",
                    JoinStatements = new List<string>() { @"JOIN CarrierAccount cust WITH (NOLOCK) ON cust.CarrierAccountID = ts.SupplierID
                                                     JOIN CarrierProfile custProf on cust.ProfileID = custProf.ProfileID" }
                });
        }

        private static void FillMeasureFieldsConfig()
        {
            s_AllMeasureFieldsConfig = new Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig>();

            throw new NotImplementedException();
        }

    }
}
