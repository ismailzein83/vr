using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;
using System.Globalization;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RPZoneCodeGroupDataManager : RoutingDataManager, IRPZoneCodeGroupDataManager
    {
        private readonly string[] columns = { "ZoneId", "IsSale", "CodeGroups" };

        #region Public Methods

        public void ApplyZoneCodeGroupsForDB(object preparedZoneCodeGroups)
        {
            InsertBulkToTable(preparedZoneCodeGroups as BaseBulkInsertInfo);
        }

        public Dictionary<bool, Dictionary<long, HashSet<string>>> GetZoneCodeGroups()
        {
            Dictionary<bool, Dictionary<long, HashSet<string>>> result = new Dictionary<bool, Dictionary<long, HashSet<string>>>();

            ExecuteReaderText(query_GetZoneCodeGroups, (reader) =>
            {
                while (reader.Read())
                {
                    bool isSale = (bool)reader["IsSale"];
                    long zoneId = (long)reader["ZoneId"];
                    string codeGroupsAsString = reader["CodeGroups"] as string;

                    HashSet<string> codeGroups = codeGroupsAsString.Split(',').ToHashSet();

                    Dictionary<long, HashSet<string>> zoneCodeGroupsByZoneId = result.GetOrCreateItem(isSale);
                    zoneCodeGroupsByZoneId.Add(zoneId, codeGroups);
                }
            }, null);

            return result;
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ZoneCodeGroup record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.ZoneId, record.IsSale ? 1 : 0, string.Join<string>(",", record.CodeGroups));
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[ZoneCodeGroup]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        #endregion



        #region Queries

        const string query_GetZoneCodeGroups = @"SELECT ZoneId, IsSale, CodeGroups
                                                    FROM    [dbo].[ZoneCodeGroup] with(nolock)";


        #endregion
    }
}
