using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CodeMatchesDataManager : RoutingDataManager, ICodeMatchesDataManager
    {

        public void ApplyCodeMatchesForDB(object preparedCodeMatches)
        {
            InsertBulkToTable(preparedCodeMatches as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CodeMatch]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.CodeMatches record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.CodePrefix, record.Code, Vanrise.Common.Serializer.Serialize(record.SupplierCodeMatches, true));
        }

        #region Queries

        const string query_GetCodeMatchesByZone = @"                                                       
                                          SELECT  cm.Code, 
                                                  cm.Content, 
                                                  sz.SaleZoneID
                                          FROM    [dbo].[CodeMatch] cm with(nolock)
                                          join    CodeSaleZone sz on sz.code = cm.code 
                                          JOIN    @ZoneList z ON z.ID = sz.SaleZoneID";

        #endregion


        public IEnumerable<RPCodeMatches> GetCodeMatches(IEnumerable<long> saleZoneIds)
        {
            DataTable dtZoneIds = BuildZoneIdsTable(new HashSet<long>(saleZoneIds));
            return GetItemsText(query_GetCodeMatchesByZone, RPCodeMatchesMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ZoneList", SqlDbType.Structured);
                dtPrm.TypeName = "LongIDType";
                dtPrm.Value = dtZoneIds;
                cmd.Parameters.Add(dtPrm);

            });
        }
        DataTable BuildZoneIdsTable(HashSet<long> zoneIds)
        {
            DataTable dtZoneInfo = new DataTable();
            dtZoneInfo.Columns.Add("ZoneID", typeof(Int64));
            dtZoneInfo.BeginLoadData();
            foreach (var z in zoneIds)
            {
                DataRow dr = dtZoneInfo.NewRow();
                dr["ZoneID"] = z;
                dtZoneInfo.Rows.Add(dr);
            }
            dtZoneInfo.EndLoadData();
            return dtZoneInfo;
        }
        RPCodeMatches RPCodeMatchesMapper(IDataReader reader)
        {
            return new RPCodeMatches()
            {
                Code = reader["Code"] as string,
                SupplierCodeMatches = reader["Content"] != null ? Vanrise.Common.Serializer.Deserialize<List<SupplierCodeMatchWithRate>>(reader["Content"].ToString()) : null,
                SaleZoneId = (long)reader["SaleZoneId"]
            };
        }

    }
}
