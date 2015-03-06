using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class ZoneDataManager : BaseTOneDataManager, IZoneDataManager
    {
        public void LoadZonesInfo(DateTime effectiveTime, bool isFuture, List<Entities.CarrierAccountInfo> activeSuppliers, int batchSize, Action<List<Entities.ZoneInfo>> onBatchAvailable)
        {
            DataTable dtActiveSuppliers = CarrierDataManager.BuildCarrierAccountInfoTable(activeSuppliers);
            List<ZoneInfo> zones = new List<ZoneInfo>();
            ExecuteReaderSPCmd("[BEntity].[sp_Zone_GetZoneInfoForActiveSuppliers]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        ZoneInfo zoneInfo = ZoneInfoMapper(reader);
                        zones.Add(zoneInfo);
                        if(zones.Count >= batchSize)
                        {
                            onBatchAvailable(zones);
                            zones = new List<ZoneInfo>();
                        }
                    } 
                    if (zones.Count >= 0)
                    {
                        onBatchAvailable(zones);
                        zones = new List<ZoneInfo>();
                    }
                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ActiveSuppliersInfo", SqlDbType.Structured);
                    dtPrm.Value = dtActiveSuppliers;
                    cmd.Parameters.Add(dtPrm);

                    cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveTime));
                    cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                });
        }
        
        public List<ZoneInfo> GetZones(string supplierId, string nameFilter)
        {
            return GetItemsSP("[BEntity].[sp_Zone_GetFilteredBySupplierId]", ZoneInfoMapper, supplierId, nameFilter);
         
        }
        
        #region Private Methods

        private ZoneInfo ZoneInfoMapper(IDataReader reader)
        {
            return new ZoneInfo
            {
                ZoneId = (int)reader["ZoneID"],
                Name = reader["Name"] as string
            };
        }

        #endregion
    }
}
