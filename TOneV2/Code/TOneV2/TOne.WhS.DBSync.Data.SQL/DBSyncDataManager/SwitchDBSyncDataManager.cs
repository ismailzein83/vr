using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.Switch);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SwitchDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySwitchesToTemp(IEnumerable<Switch> switches)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("Settings", typeof(string));
            dt.BeginLoadData();
            foreach (var item in switches)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.Name;
                row[index++] = item.SourceId;
                row[index++] = item.Settings != null ? Vanrise.Common.Serializer.Serialize(item.Settings) : null;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, Switch> GetSwitches(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID]  ,[Name], [SourceID], [Settings] FROM {0} where sourceid is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), SwitchMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public void FixSwitchIds()
        {
            ExecuteNonQueryText(query_FixSwitchIds, null);
        }

        public Switch SwitchMapper(IDataReader reader)
        {
            string settings = reader["Settings"] as string;
            return new Switch
            {
                SwitchId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SourceId = reader["SourceID"] as string,
                Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<SwitchSettings>(settings) : null
            };
        }

        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetTableName()
        {
            return _TableName;
        }

        public string GetSchema()
        {
            return _Schema;
        }

        #region queries
        const string query_FixSwitchIds = @"
                                            select    st.ID newID
                                                                ,s.ID
                                                                ,st.[Name]
                                                                ,st.[SourceID]
                                                                ,st.[Settings] 
                                into        #TempSwitches_ToUpdate 
                                FROM   [TOneWhS_BE].[Switch] s 
                                join        [TOneWhS_BE].[Switch_Temp] st on s.SourceID = st.SourceID

                                select    st.ID
                                                                ,st.[Name]
                                                                ,st.[SourceID]
                                                                ,st.[Settings] 
                                into        #TempSwitches_New 
                                FROM   [TOneWhS_BE].[Switch] s 
                                right join [TOneWhS_BE].[Switch_Temp] st on s.SourceID = st.SourceID 
                                where   s.ID is null

                                if exists(SELECT * from #TempSwitches_ToUpdate WHERE newID != ID)
                                begin
                                                truncate table [TOneWhS_BE].[Switch_Temp]

                                                ---Fix IDs
                                                set identity_insert [TOneWhS_BE].[Switch_Temp] ON

                                                insert into [TOneWhS_BE].[Switch_Temp](ID,[Name],[SourceID],[Settings] )
                                                select    ID,[Name],[SourceID],[Settings] 
                                                from      #TempSwitches_ToUpdate 

                                               set identity_insert [TOneWhS_BE].[Switch_Temp] OFF
                
                                                insert into [TOneWhS_BE].[Switch_Temp]([Name],[SourceID],[Settings] )

                                                select [Name],[SourceID],[Settings] from #TempSwitches_New               
                                end";
        #endregion
    }
}
