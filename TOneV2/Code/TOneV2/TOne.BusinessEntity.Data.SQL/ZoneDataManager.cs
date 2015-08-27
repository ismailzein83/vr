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
            DataTable dtActiveSuppliers = CarrierAccountDataManager.BuildCarrierAccountInfoTable(activeSuppliers);
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
        
        //public List<ZoneInfo> GetZones(string supplierId, string nameFilter)
        //{
        //    return GetItemsSP("[BEntity].[sp_Zone_GetFilteredBySupplierId]", ZoneInfoMapper, supplierId, nameFilter);
         
        //}
        public List<ZoneInfo> GetSupplierZones(string supplierId, string nameFilter, DateTime whenDate)
        {
            return GetItemsSP("[BEntity].[sp_Zone_GetFilteredBySupplier]", ZoneInfoMapper, supplierId, nameFilter, whenDate);

        }
        public List<ZoneInfo> GetCustomerZones(string customerId, string nameFilter, DateTime whenDate)
        {
            return GetItemsSP("[BEntity].[sp_Zone_GetFilteredByCustomer]", ZoneInfoMapper, customerId, nameFilter, whenDate);

        }

        public string GetZoneName(int zoneId)
        {
            return ExecuteScalarSP("[BEntity].[sp_Zone_GetName]", zoneId) as string;
        }

        public List<ZoneInfo> GetZoneList(IEnumerable<int> zonesIds)
        {
            DataTable dtZones = BuildZoneInfoTable(zonesIds);

             return GetItemsSPCmd("[BEntity].[sp_Zone_GetByZonesIds]",
                   ZoneInfoMapper,
                   (cmd) =>
                   {
                       var dtPrm = new SqlParameter("@ZonesIds", SqlDbType.Structured);
                       dtPrm.Value = dtZones;
                       cmd.Parameters.Add(dtPrm);
                       
                   });
           

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

        internal static DataTable BuildZoneInfoTable(IEnumerable<int> zonesIds)
        {
            DataTable dtZonwInfo = new DataTable();
            dtZonwInfo.Columns.Add("ID", typeof(int));
            dtZonwInfo.BeginLoadData();
            foreach (var z in zonesIds)
            {
                DataRow dr = dtZonwInfo.NewRow();
                dr["ID"] = z;
                dtZonwInfo.Rows.Add(dr);
            }
            dtZonwInfo.EndLoadData();
            return dtZonwInfo;
        }


        #endregion


        public Dictionary<int, Zone> GetAllZones()
        {
            Dictionary<int, Zone> allZones = new Dictionary<int, Zone>();
            ExecuteReaderSP("[BEntity].[sp_Zone_All]", (reader) =>
            {
                while (reader.Read())
                {
                    Zone zone = ZoneMapper(reader);
                    allZones.Add(zone.ZoneId, zone);
                }
            });

            return allZones;
        }
        private Zone ZoneMapper(IDataReader reader)
        {
            return new Zone
            {
                ZoneId = (int)reader["ZoneID"],
                CodeGroupId = reader["CodeGroupId"] as string,
                CodeGroupName = reader["CodeGroupName"] as string,
                SupplierID = reader["SupplierID"] as string,
                Name = reader["Name"] as string,
                ServiceFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate")
            };
        }

        //public List<ZoneInfo> GetZonesBySupplierID(string supplierID)
        //{
        //    return GetItemsSP("BEntity.sp_Zone_GetBySupplierID", ZoneInfoMapper, supplierID);
        //}
    }
}
