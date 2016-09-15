using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceSupplierZoneServiceDataManager : BaseSQLDataManager
    {
        public SourceSupplierZoneServiceDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceRate> GetSourceSupplierZoneServices(bool isSaleRate)
        {
            return GetItemsText(query_getSourceRates, SourceRateMapper, null);
        }


        public void LoadSourceItems(bool isSaleRate, Action<SourceRate> itemToAdd)
        {
            ExecuteReaderText(query_getSourceRates, (reader) =>
            {
                while (reader.Read())
                    itemToAdd(SourceRateMapper(reader));

            }, null);
        }

        private SourceRate SourceRateMapper(IDataReader reader)
        {
            return new SourceRate()
            {
                SourceId = reader["RateID"].ToString(),
                ZoneId = GetReaderValue<int?>(reader, "ZoneID"),
                BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                ServicesFlag = GetReaderValue<Int16>(reader, "ServicesFlag")
            };
        }

        const string query_getSourceRates = @"SELECT    Rate.RateID RateID, Rate.ZoneID ZoneID,
                                                        Rate.BeginEffectiveDate BeginEffectiveDate, Rate.EndEffectiveDate EndEffectiveDate,
                                                        Rate.ServicesFlag
                                                        FROM Rate WITH (NOLOCK) INNER JOIN
                                                        Zone WITH (NOLOCK) ON Rate.ZoneID = Zone.ZoneID INNER JOIN
                                                        PriceList WITH (NOLOCK) ON Rate.PriceListID = PriceList.PriceListID 
                                                        where Zone.SupplierID <> 'SYS'";
    }
}
