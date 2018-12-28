using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRMailMessageTypeDataManager : IVRMailMessageTypeDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_MailMessageType";
		static string TABLE_ALIAS = "vrMailMessageType";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Contructors
		static VRMailMessageTypeDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "MailMessageType",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreMailMessageTypeUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public List<VRMailMessageType> GetMailMessageTypes()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(VRMailMessageTypeMapper);
		}

		public bool Insert(VRMailMessageType vrMailMessageTypeItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(vrMailMessageTypeItem.Name);
			insertQuery.Column(COL_ID).Value(vrMailMessageTypeItem.VRMailMessageTypeId);
			insertQuery.Column(COL_Name).Value(vrMailMessageTypeItem.Name);
			if (vrMailMessageTypeItem.Settings != null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrMailMessageTypeItem.Settings));
			else
				insertQuery.Column(COL_Settings).Null();
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(VRMailMessageType vrMailMessageTypeItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.NotEqualsCondition(COL_ID).Value(vrMailMessageTypeItem.VRMailMessageTypeId);
			ifNotExist.EqualsCondition(COL_Name).Value(vrMailMessageTypeItem.Name);
			updateQuery.Column(COL_Name).Value(vrMailMessageTypeItem.Name);
			if (vrMailMessageTypeItem.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrMailMessageTypeItem.Settings));
			else
				updateQuery.Column(COL_Settings).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(vrMailMessageTypeItem.VRMailMessageTypeId);
			return queryContext.ExecuteNonQuery() > 0;
		}

		public void GenerateScript(List<VRMailMessageType> mailTypes, Action<string, string> addEntityScript)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private VRMailMessageType VRMailMessageTypeMapper(IRDBDataReader reader)
		{
			return new VRMailMessageType
			{
				VRMailMessageTypeId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				Settings = Vanrise.Common.Serializer.Deserialize<VRMailMessageTypeSettings>(reader.GetString(COL_Settings)),
			};
		}

		#endregion

	}
}
