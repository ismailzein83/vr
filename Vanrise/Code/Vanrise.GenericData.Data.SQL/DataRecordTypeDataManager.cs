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
    public class DataRecordTypeDataManager : BaseSQLDataManager, IDataRecordTypeDataManager
    {
        public DataRecordTypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }
        #region Public Methods
        public List<DataRecordType> GetDataRecordTypes()
        {
            return GetItemsSP("genericdata.sp_DataRecordType_GetAll", DataRecordTypeMapper);
        }

        public bool AreDataRecordTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.DataRecordType", ref updateHandle);
        }
        public bool UpdateDataRecordType(DataRecordType dataRecordType)
        {
            string serializedFields = dataRecordType.Fields != null ? Vanrise.Common.Serializer.Serialize(dataRecordType.Fields) : null;
            string serializedExtraFieldsEvaluator = dataRecordType.ExtraFieldsEvaluator != null ? Vanrise.Common.Serializer.Serialize(dataRecordType.ExtraFieldsEvaluator) : null;
            string serializedSettings = dataRecordType.Settings != null ? Vanrise.Common.Serializer.Serialize(dataRecordType.Settings) : null;

            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_DataRecordType_Update", dataRecordType.DataRecordTypeId, dataRecordType.Name, dataRecordType.ParentId, dataRecordType.DevProjectId, serializedFields, serializedExtraFieldsEvaluator, serializedSettings);
            return (recordesEffected > 0);
        }

        public bool AddDataRecordType(DataRecordType dataRecordType)
        {
            string serializedFields = dataRecordType.Fields != null ? Vanrise.Common.Serializer.Serialize(dataRecordType.Fields) : null;
            string serializedExtraFieldsEvaluator = dataRecordType.ExtraFieldsEvaluator != null ? Vanrise.Common.Serializer.Serialize(dataRecordType.ExtraFieldsEvaluator) : null;
            string serializedSettings = dataRecordType.Settings != null ? Vanrise.Common.Serializer.Serialize(dataRecordType.Settings) : null;

            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_DataRecordType_Insert", dataRecordType.DataRecordTypeId, dataRecordType.Name, dataRecordType.ParentId,dataRecordType.DevProjectId, serializedFields, serializedExtraFieldsEvaluator, serializedSettings);
            return (recordesEffected > 0);
        }

        public void SetDataRecordTypeCacheExpired()
        {
            ExecuteNonQuerySP("genericdata.sp_DataRecordType_SetCacheExpired");
        }
        public void GenerateScript(List<DataRecordType> dataRecordTypes, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var dataRecordType in dataRecordTypes)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}','{5}')", dataRecordType.DataRecordTypeId, dataRecordType.Name, dataRecordType.ParentId, Serializer.Serialize(dataRecordType.Fields), Serializer.Serialize(dataRecordType.ExtraFieldsEvaluator), Serializer.Serialize(dataRecordType.Settings));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings]))
merge	[genericdata].[DataRecordType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
	values(s.[ID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings]);", scriptBuilder);
            addEntityScript("[genericdata].[DataRecordType]", script);
        }

        #endregion

        #region Mappers

        DataRecordType DataRecordTypeMapper(IDataReader reader)
        {
            return new DataRecordType
            {
                DataRecordTypeId = GetReaderValue<Guid>(reader, "ID"),
                DevProjectId = GetReaderValue<Guid?>(reader, "DevProjectID"),
                Name = reader["Name"] as string,
                ParentId = GetReaderValue<Guid?>(reader, "ParentID"),
                Fields = Vanrise.Common.Serializer.Deserialize<List<DataRecordField>>(reader["Fields"] as string),
                ExtraFieldsEvaluator = reader["ExtraFieldsEvaluator"] != DBNull.Value ? Vanrise.Common.Serializer.Deserialize<DataRecordTypeExtraField>(reader["ExtraFieldsEvaluator"] as string) : null,
                Settings = reader["Settings"] != DBNull.Value ? Vanrise.Common.Serializer.Deserialize<DataRecordTypeSettings>(reader["Settings"] as string) : null,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                LastModifiedTime = GetReaderValue<DateTime>(reader, "LastModifiedTime")
            };
        }

        #endregion
    }
}