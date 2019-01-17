using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRDynamicAPIDataManager : IVRDynamicAPIDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRDynamicAPI";
		static string TABLE_ALIAS = "vrDynamicAPI";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_ModuleId = "ModuleId";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedBy = "LastModifiedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRDynamicAPIDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_ModuleId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRDynamicAPI",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreVRDynamicAPIsUpdated(ref object updateHandle)
		{
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

		public List<VRDynamicAPI> GetVRDynamicAPIs()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name,RDBSortDirection.ASC);
			return queryContext.GetItems(VRDynamicAPIMapper);
		}

		public bool Insert(VRDynamicAPI vrDynamicAPI)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(vrDynamicAPI.Name);
			ifNotExist.EqualsCondition(COL_ModuleId).Value(vrDynamicAPI.ModuleId);
            insertQuery.Column(COL_ID).Value(vrDynamicAPI.VRDynamicAPIId);
            insertQuery.Column(COL_Name).Value(vrDynamicAPI.Name);
			insertQuery.Column(COL_ModuleId).Value(vrDynamicAPI.ModuleId);
            if(vrDynamicAPI.Settings != null)
			    insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrDynamicAPI.Settings));
			insertQuery.Column(COL_CreatedBy).Value(vrDynamicAPI.CreatedBy);
			insertQuery.Column(COL_LastModifiedBy).Value(vrDynamicAPI.LastModifiedBy);
			return queryContext.ExecuteNonQuery() > 0;

        }

		public bool Update(VRDynamicAPI vrDynamicAPI)
		{
			var queryConext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryConext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(vrDynamicAPI.VRDynamicAPIId);
			ifNotExist.EqualsCondition(COL_Name).Value(vrDynamicAPI.Name);
			ifNotExist.EqualsCondition(COL_ModuleId).Value(vrDynamicAPI.ModuleId);
			updateQuery.Column(COL_Name).Value(vrDynamicAPI.Name);
			updateQuery.Column(COL_ModuleId).Value(vrDynamicAPI.ModuleId);
            if (vrDynamicAPI.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrDynamicAPI.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            updateQuery.Column(COL_LastModifiedBy).Value(vrDynamicAPI.LastModifiedBy);
			updateQuery.Where().EqualsCondition(COL_ID).Value(vrDynamicAPI.VRDynamicAPIId);
			return queryConext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		VRDynamicAPI VRDynamicAPIMapper(IRDBDataReader reader)
		{
			return new VRDynamicAPI
			{
				VRDynamicAPIId =reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				ModuleId = reader.GetGuid(COL_ModuleId),
				Settings = Vanrise.Common.Serializer.Deserialize<VRDynamicAPISettings>(reader.GetString(COL_Settings)),
				CreatedTime =reader.GetDateTime(COL_CreatedTime),
				CreatedBy =reader.GetInt(COL_CreatedBy),
				LastModifiedTime = reader.GetDateTime(COL_LastModifiedTime),
				LastModifiedBy = reader.GetInt(COL_LastModifiedBy)
			};
		}
		#endregion

	}
}
