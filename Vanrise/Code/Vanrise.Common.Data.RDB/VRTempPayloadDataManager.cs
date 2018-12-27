using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRTempPayloadDataManager : IVRTempPayloadDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRTempPayload";
		static string TABLE_ALIAS = "vrTempPayload";
		const string COL_ID = "ID";
		const string COL_Settings = "Settings";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";

		#endregion

		#region Constructors
		static VRTempPayloadDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRTempPayload",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool DeleteVRTempPayload(Guid vrTempPayloadId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var deleteQuery = queryContext.AddDeleteQuery();
			deleteQuery.FromTable(TABLE_NAME);
			deleteQuery.Where().EqualsCondition(COL_ID).Value(vrTempPayloadId);
			return queryContext.ExecuteNonQuery() > 0;
		}

		public VRTempPayload GetVRTempPayload(Guid vrTempPayloadId, DateTime? deleteBeforeDate)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Where().EqualsCondition(COL_ID).Value(vrTempPayloadId);

			var deleteQuery = queryContext.AddDeleteQuery();
			deleteQuery.FromTable(TABLE_NAME);
			deleteQuery.Where().LessThanCondition(COL_CreatedTime).Value(deleteBeforeDate.Value);
			return queryContext.GetItem(VRTempPayloadMapper);
		}

		public bool Insert(VRTempPayload vrTempPayload)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_ID).Value(vrTempPayload.VRTempPayloadId);
			if (vrTempPayload.Settings != null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrTempPayload.Settings));
			else
				insertQuery.Column(COL_Settings).Null();
			insertQuery.Column(COL_CreatedBy).Value(vrTempPayload.CreatedBy);
			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private	VRTempPayload VRTempPayloadMapper(IRDBDataReader reader)
		{
			return new VRTempPayload
			{
				VRTempPayloadId = reader.GetGuid(COL_ID),
				Settings = Vanrise.Common.Serializer.Deserialize<VRTempPayloadSettings>(reader.GetString(COL_Settings)),
				CreatedBy =reader.GetInt(COL_CreatedBy),
				CreatedTime = reader.GetDateTime(COL_CreatedTime)
			};

		}
		#endregion

	}
}
