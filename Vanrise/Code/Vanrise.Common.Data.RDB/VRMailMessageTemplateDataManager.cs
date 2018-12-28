using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	class VRMailMessageTemplateDataManager : IVRMailMessageTemplateDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_MailMessageTemplate";
		static string TABLE_ALIAS = "vrMailMessageTemplate";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_MessageTypeID = "MessageTypeID";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Contructors
		static VRMailMessageTemplateDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_MessageTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "MailMessageTemplate",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime
			});
		}
		#endregion

		#region Public Methods
		public List<VRMailMessageTemplate> GetMailMessageTemplates()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(VRMailMessageTemplateMapper);
		}

		public bool AreMailMessageTemplateUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public bool Insert(VRMailMessageTemplate vrMailMessageTemplateItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(vrMailMessageTemplateItem.Name);
			insertQuery.Column(COL_ID).Value(vrMailMessageTemplateItem.VRMailMessageTemplateId);
			insertQuery.Column(COL_Name).Value(vrMailMessageTemplateItem.Name);
			insertQuery.Column(COL_MessageTypeID).Value(vrMailMessageTemplateItem.VRMailMessageTypeId);
			if (vrMailMessageTemplateItem.Settings != null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrMailMessageTemplateItem.Settings));
			else
				insertQuery.Column(COL_Settings).Null();
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(VRMailMessageTemplate vrMailMessageTemplateItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
		
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.NotEqualsCondition(COL_ID).Value(vrMailMessageTemplateItem.VRMailMessageTemplateId);
			ifNotExist.EqualsCondition(COL_Name).Value(vrMailMessageTemplateItem.Name);

			updateQuery.Column(COL_Name).Value(vrMailMessageTemplateItem.Name);
			if (vrMailMessageTemplateItem.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrMailMessageTemplateItem.Settings));
			else
				updateQuery.Column(COL_Settings).Null();
			updateQuery.Column(COL_MessageTypeID).Value(vrMailMessageTemplateItem.VRMailMessageTypeId);
			updateQuery.Where().EqualsCondition(COL_ID).Value(vrMailMessageTemplateItem.VRMailMessageTemplateId);
			return queryContext.ExecuteNonQuery() > 0;

		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private VRMailMessageTemplate VRMailMessageTemplateMapper(IRDBDataReader reader)
		{
			return new VRMailMessageTemplate
			{
				VRMailMessageTemplateId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				VRMailMessageTypeId = reader.GetGuid(COL_MessageTypeID),
				Settings = Vanrise.Common.Serializer.Deserialize<VRMailMessageTemplateSettings>(reader.GetString(COL_Settings)),
				CreatedTime = reader.GetDateTime(COL_CreatedTime)
			};
		}
		#endregion

	}
}
