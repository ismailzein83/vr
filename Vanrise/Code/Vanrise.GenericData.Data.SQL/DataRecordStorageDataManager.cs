using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class DataRecordStorageDataManager : BaseSQLDataManager, IDataRecordStorageDataManager
    {
        public DataRecordStorageDataManager() : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods

        public IEnumerable<DataRecordStorage> GetDataRecordStorages()
        {
            return GetItemsSP("genericdata.sp_DataRecordStorage_GetALL", DataRecordStorageMapper);
        }

        public bool AddDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            int affectedRows = ExecuteNonQuerySP
                (
                    "genericdata.sp_DataRecordStorage_Insert",
                    dataRecordStorage.DataRecordStorageId,
                    dataRecordStorage.Name,
                    dataRecordStorage.DataRecordTypeId,
                    dataRecordStorage.DataStoreId,
                    Vanrise.Common.Serializer.Serialize(dataRecordStorage.Settings),
                    DateTime.Now
                );
            
            if (affectedRows == 1) {
                return true;
            }
            else {
                return false;
            }
        }

        public bool UpdateDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            int affectedRows = ExecuteNonQuerySP
                (
                    "genericdata.sp_DataRecordStorage_Update",
                    dataRecordStorage.DataRecordStorageId,
                    dataRecordStorage.Name,
                    dataRecordStorage.DataRecordTypeId,
                    dataRecordStorage.DataStoreId,
                    Vanrise.Common.Serializer.Serialize(dataRecordStorage.Settings)
                );
            return (affectedRows == 1);
        }

        public bool AreDataRecordStoragesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.DataRecordStorage", ref updateHandle);
        }
        public void GenerateScript(List<DataRecordStorage> dataRecordStorages, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var dataRecordStorage in dataRecordStorages)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}','{5}')", dataRecordStorage.DataRecordStorageId, dataRecordStorage.Name, dataRecordStorage.DataRecordTypeId, dataRecordStorage.DataStoreId, Serializer.Serialize(dataRecordStorage.Settings), Serializer.Serialize(dataRecordStorage.State));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State]))
merge	[genericdata].[DataRecordStorage] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DataRecordTypeID] = s.[DataRecordTypeID],[DataStoreID] = s.[DataStoreID],[Settings] = s.[Settings],[State] = s.[State]
when not matched by target then
	insert([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
	values(s.[ID],s.[Name],s.[DataRecordTypeID],s.[DataStoreID],s.[Settings],s.[State]);", scriptBuilder);
            addEntityScript("[genericdata].[DataRecordStorage]", script);
        }
        #endregion

        #region Mappers

        DataRecordStorage DataRecordStorageMapper(IDataReader reader)
        {
            return new DataRecordStorage() {
                DataRecordStorageId = GetReaderValue<Guid>(reader,"ID"),
                DevProjectId = GetReaderValue<Guid?>(reader, "DevProjectID"),
                Name = (string)reader["Name"],
                DataRecordTypeId = GetReaderValue<Guid>(reader,"DataRecordTypeID"),
                DataStoreId = GetReaderValue<Guid>(reader,"DataStoreID"),
                Settings = Vanrise.Common.Serializer.Deserialize<DataRecordStorageSettings>((string)reader["Settings"]),
                State = (GetReaderValue<string>(reader, "State") != null) ? Vanrise.Common.Serializer.Deserialize<DataRecordStorageState>((string)reader["State"]) : null
            };
        }

       

        #endregion
    }
}
