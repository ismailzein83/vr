using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class CodeMatchesDataManager : RoutingDataManager, ICodeMatchesDataManager
    {
        readonly string[] columns = { "CodePrefix", "Code", "Content" };
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
                ColumnNames = columns
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(TOne.WhS.Routing.Entities.CodeMatches record, object dbApplyStream)
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
                                          where   sz.SaleZoneId between @FromZoneId and @ToZoneId";

        #endregion


        public IEnumerable<RPCodeMatches> GetCodeMatches(long fromZoneId, long toZoneId)
        {
            return GetItemsText(query_GetCodeMatchesByZone, RPCodeMatchesMapper, (cmd) =>
            {

                var dtPrm = new SqlParameter("@FromZoneId", SqlDbType.BigInt);
                dtPrm.Value = fromZoneId;
                cmd.Parameters.Add(dtPrm);
                dtPrm = new SqlParameter("@ToZoneId", SqlDbType.BigInt);
                dtPrm.Value = toZoneId;
                cmd.Parameters.Add(dtPrm);
            });
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
