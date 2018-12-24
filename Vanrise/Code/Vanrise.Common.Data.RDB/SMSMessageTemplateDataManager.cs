using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class SMSMessageTemplateDataManager : ISMSMessageTemplateDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_SMSMessageTemplate";
		static string TABLE_ALIAS = "vrSMSMessageTemplate";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_SMSMessageTypeId = "SMSMessageTypeId";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedBy = "LastModifiedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static SMSMessageTemplateDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_SMSMessageTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "SMSMessageTemplate",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreSMSMessageTemplateUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public List<SMSMessageTemplate> GetSMSMessageTemplates()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name,RDBSortDirection.ASC);
			return queryContext.GetItems(SMSMessageTemplateMapper);
		}

		public bool Insert(SMSMessageTemplate smsMessageTemplateItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(smsMessageTemplateItem.Name);

			insertQuery.Column(COL_ID).Value(smsMessageTemplateItem.SMSMessageTemplateId);
			insertQuery.Column(COL_Name).Value(smsMessageTemplateItem.Name);
			insertQuery.Column(COL_SMSMessageTypeId).Value(smsMessageTemplateItem.SMSMessageTypeId);
			if (smsMessageTemplateItem.Settings != null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(smsMessageTemplateItem.Settings));
			else
				insertQuery.Column(COL_Settings).Null();
			insertQuery.Column(COL_CreatedBy).Value(smsMessageTemplateItem.CreatedBy);
			insertQuery.Column(COL_LastModifiedBy).Value(smsMessageTemplateItem.LastModifiedBy);

			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(SMSMessageTemplate smsMessageTemplateItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);

			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(smsMessageTemplateItem.SMSMessageTemplateId);
			ifNotExist.EqualsCondition(COL_Name).Value(smsMessageTemplateItem.Name);

			updateQuery.Column(COL_Name).Value(smsMessageTemplateItem.Name);
			updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(smsMessageTemplateItem.Settings));
			updateQuery.Column(COL_SMSMessageTypeId).Value(smsMessageTemplateItem.SMSMessageTypeId);
			updateQuery.Column(COL_LastModifiedBy).Value(smsMessageTemplateItem.LastModifiedBy);

			updateQuery.Where().EqualsCondition(COL_ID).Value(smsMessageTemplateItem.SMSMessageTemplateId);
			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_SMSMessageTemplate", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}

		#endregion

		#region Mappers
		SMSMessageTemplate SMSMessageTemplateMapper(IRDBDataReader reader)
		{
			return new SMSMessageTemplate
			{
				SMSMessageTemplateId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				SMSMessageTypeId =reader.GetGuid(COL_SMSMessageTypeId),
				Settings = Vanrise.Common.Serializer.Deserialize<SMSMessageTemplateSettings>(reader.GetString(COL_Settings)),
				CreatedBy = reader.GetInt(COL_CreatedBy),
				CreatedTime = reader.GetDateTime(COL_CreatedTime),
				LastModifiedBy = reader.GetInt(COL_LastModifiedBy),
				LastModifiedTime = reader.GetDateTime(COL_LastModifiedTime)
			};
		}

		#endregion
	}
}
