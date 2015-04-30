using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class TariffDataManager : BaseTOneDataManager, ITariffDataManager
    {
        private Tariff TariffMapper(IDataReader reader)
        {
            try
            {
                Tariff tariff = new Tariff();

                tariff.TariffID = int.Parse(reader["TariffID"].ToString());
                tariff.ZoneId = int.Parse(reader["ZoneID"].ToString());// GetReaderValue<int>(reader, "ZoneID"),
                tariff.SupplierId = reader["SupplierID"] as string;
                tariff.CustomerId = reader["CustomerID"] as string;
                tariff.CallFee = GetReaderValue<decimal>(reader, "CallFee");
                tariff.FirstPeriodRate = GetReaderValue<decimal>(reader, "FirstPeriodRate");
                tariff.FirstPeriod = int.Parse(reader["FirstPeriod"].ToString()); //GetReaderValue<int>(reader, "FirstPeriod");
                tariff.RepeatFirstPeriod = reader["RepeatFirstPeriod"] as string == "Y";
                tariff.FractionUnit = int.Parse(reader["FractionUnit"].ToString());// GetReaderValue<int>(reader, "FractionUnit");
                tariff.BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate");
                tariff.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate");
                return tariff;

            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return new Tariff();
            //return new Tariff
            //{
            //    TariffID = int.Parse(reader["TariffID"].ToString()),
            //    ZoneId = int.Parse(reader["ZoneID"].ToString()),// GetReaderValue<int>(reader, "ZoneID"),
            //    SupplierId = reader["SupplierID"] as string,
            //    CustomerId = reader["CustomerID"] as string,
            //    CallFee = GetReaderValue<decimal>(reader, "CallFee"),
            //    FirstPeriodRate = GetReaderValue<decimal>(reader, "FirstPeriodRate"),
            //    FirstPeriod = GetReaderValue<int>(reader, "FirstPeriod"),
            //    RepeatFirstPeriod = reader["RepeatFirstPeriod"] as string == "Y",
            //    FractionUnit = GetReaderValue<int>(reader, "FractionUnit"),
            //    BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate"),
            //    EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate")
            //};
        }

        public List<Tariff> GetTariff(string customerId, int zoneId, DateTime when)
        {
            return GetItemsSP("BEntity.sp_Tariff_GetTariffs", TariffMapper, zoneId, customerId, when);
        }
    }
}
