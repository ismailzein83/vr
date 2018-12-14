using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class StatusDefinitionDataManager : IStatusDefinitionDataManager
	{
		#region Local Variables

		static string TABLE_NAME = "common_StatusDefinition";
		static string TABLE_ALIAS = "statusDefinition";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_BusinessEntityDefinitionID = "BusinessEntityDefinitionID";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedBy = "LastModifiedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";

		#endregion

		#region Constructors

		static StatusDefinitionDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_BusinessEntityDefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "StatusDefinition",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}

		#endregion

		#region Public Methods
		public bool AreStatusDefinitionUpdated(ref object updateHandle)
		{
			throw new NotImplementedException();
		}

		public List<StatusDefinition> GetStatusDefinition()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Columns(COL_ID, COL_Name, COL_Settings, COL_BusinessEntityDefinitionID, COL_CreatedTime, COL_CreatedBy, COL_LastModifiedBy, COL_LastModifiedTime);
			return queryContext.GetItems(StatusDefinitionMapper);
		}

		public bool Insert(StatusDefinition statusDefinitionItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.EqualsCondition(TABLE_ALIAS, COL_Name).Value(statusDefinitionItem.Name);
			ifNotExist.EqualsCondition(TABLE_ALIAS, COL_BusinessEntityDefinitionID).Value(statusDefinitionItem.BusinessEntityDefinitionId);
			insertQuery.Column(COL_ID).Value(statusDefinitionItem.StatusDefinitionId);
			insertQuery.Column(COL_Name).Value(statusDefinitionItem.Name);
			insertQuery.Column(COL_BusinessEntityDefinitionID).Value(statusDefinitionItem.BusinessEntityDefinitionId);
			insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(statusDefinitionItem.Settings));
			if (statusDefinitionItem.CreatedBy.HasValue)
				insertQuery.Column(COL_CreatedBy).Value(statusDefinitionItem.CreatedBy.Value);
			else
				insertQuery.Column(COL_CreatedBy).Null();
			if (statusDefinitionItem.LastModifiedBy.HasValue)
				insertQuery.Column(COL_LastModifiedBy).Value(statusDefinitionItem.LastModifiedBy.Value);
			else
				insertQuery.Column(COL_LastModifiedBy).Null();
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(StatusDefinition statusDefinitionItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.NotEqualsCondition(COL_ID).Value(statusDefinitionItem.StatusDefinitionId);
			ifNotExist.EqualsCondition(COL_Name).Value(statusDefinitionItem.Name);
			ifNotExist.EqualsCondition(COL_BusinessEntityDefinitionID).Value(statusDefinitionItem.BusinessEntityDefinitionId);
			updateQuery.Column(COL_Name).Value(statusDefinitionItem.Name);
			updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(statusDefinitionItem.Settings));
			updateQuery.Column(COL_BusinessEntityDefinitionID).Value(statusDefinitionItem.BusinessEntityDefinitionId);
			if(statusDefinitionItem.LastModifiedBy.HasValue)
			updateQuery.Column(COL_LastModifiedBy).Value(statusDefinitionItem.LastModifiedBy.Value);
			else
			updateQuery.Column(COL_LastModifiedBy).Null();
			updateQuery.Where(RDBConditionGroupOperator.AND).EqualsCondition(COL_ID).Value(statusDefinitionItem.StatusDefinitionId);
			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_StatusDefinition", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}

		#endregion

		#region Mappers
		StatusDefinition StatusDefinitionMapper(IRDBDataReader reader)
		{
			StatusDefinition statusDefinition = new StatusDefinition
			{
				StatusDefinitionId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				BusinessEntityDefinitionId =reader.GetGuid(COL_BusinessEntityDefinitionID),
				Settings = reader.GetString(COL_Settings) != null ? Vanrise.Common.Serializer.Deserialize<StatusDefinitionSettings>(reader.GetString(COL_Settings)) : null,
				CreatedTime = reader.GetDateTime(COL_CreatedTime),
				CreatedBy = reader.GetNullableInt(COL_CreatedBy),
				LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
				LastModifiedTime =reader.GetNullableDateTime(COL_LastModifiedTime),
			};
			return statusDefinition;
		}
		#endregion




	}
}
