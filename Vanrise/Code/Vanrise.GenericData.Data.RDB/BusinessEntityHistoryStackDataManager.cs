using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.RDB
{
	class BusinessEntityHistoryStackDataManager : IBusinessEntityHistoryStackDataManager
	{
		#region RDB

		static string TABLE_NAME = "genericdata_BusinessEntityHistoryStack";
		static string TABLE_ALIAS = "historyStack";
		const string COL_ID = "ID";
		const string COL_BusinessEntityDefinitionID = "BusinessEntityDefinitionID";
		const string COL_BusinessEntityID = "BusinessEntityID";
		const string COL_FieldName = "FieldName";
		const string COL_StatusID = "StatusID";
		const string COL_PreviousStatusID = "PreviousStatusID";
		const string COL_StatusChangedDate = "StatusChangedDate";
		const string COL_IsDeleted = "IsDeleted";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_MoreInfo = "MoreInfo";
		const string COL_PreviousMoreInfo = "PreviousMoreInfo";


		static BusinessEntityHistoryStackDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_BusinessEntityDefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_BusinessEntityID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
			columns.Add(COL_FieldName, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
			columns.Add(COL_StatusID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_PreviousStatusID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_StatusChangedDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
			columns.Add(COL_MoreInfo, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_PreviousMoreInfo, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });

			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "genericdata",
				DBTableName = "BusinessEntityStatusHistory",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Public Methods
		public IEnumerable<BusinessEntityHistoryStack> GetFilteredBusinessEntitiesHistoryStack(DataRetrievalInput<BusinessEntityHistoryStackQuery> input)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

			var whereStatement = selectQuery.Where();
			whereStatement.EqualsCondition(COL_BusinessEntityDefinitionID).Value(input.Query.BusinessEntityDefinitionId);
			whereStatement.EqualsCondition(COL_BusinessEntityID).Value(input.Query.BusinessEntityId);

			return queryContext.GetItems(BusinessEntityStatusHistoryMapper);
		}

		public BusinessEntityHistoryStack GetLastBusinessEntityHistoryStack(Guid businessEntityDefinitionId, string businessEntityId, string fieldName)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, false);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			var whereStatement = selectQuery.Where();
			whereStatement.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
			whereStatement.EqualsCondition(COL_BusinessEntityDefinitionID).Value(businessEntityDefinitionId);
			whereStatement.EqualsCondition(COL_BusinessEntityID).Value(businessEntityId);
			whereStatement.EqualsCondition(COL_FieldName).Value(fieldName);
			selectQuery.Sort().ByColumn(COL_StatusChangedDate, RDBSortDirection.DESC);
			return queryContext.GetItem(BusinessEntityStatusHistoryMapper);
		}

		public bool Insert(Guid businessEntityDefinitionId, string businessEntityId, string fieldName, Guid statusId, Guid? previousStatusId, string moreInfo, string previousMoreInfo)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_BusinessEntityDefinitionID).Value(businessEntityDefinitionId);
			insertQuery.Column(COL_BusinessEntityID).Value(businessEntityId);
			insertQuery.Column(COL_FieldName).Value(fieldName);
			insertQuery.Column(COL_StatusID).Value(statusId);
			if (previousStatusId.HasValue)
				insertQuery.Column(COL_PreviousStatusID).Value(previousStatusId.Value);
			insertQuery.Column(COL_StatusChangedDate).DateNow();
			insertQuery.Column(COL_IsDeleted).Value(false);
			insertQuery.Column(COL_MoreInfo).Value(moreInfo);
			insertQuery.Column(COL_PreviousMoreInfo).Value(previousMoreInfo);
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool DeleteBusinessEntityHistoryStack(long id)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_IsDeleted).Value(true);
			updateQuery.Where().EqualsCondition(COL_ID).Value(id);
			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_GenericData", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		BusinessEntityHistoryStack BusinessEntityStatusHistoryMapper(IRDBDataReader reader)
		{
			return new BusinessEntityHistoryStack
			{
				StatusId = reader.GetGuid(COL_StatusID),
				StatusChangedDate = reader.GetDateTime(COL_StatusChangedDate),
				FieldName = reader.GetString(COL_FieldName),
				IsDeleted = reader.GetBoolean(COL_IsDeleted),
				PreviousStatusId = reader.GetNullableGuid(COL_PreviousStatusID),
				BusinessEntityHistoryStackId = reader.GetLong(COL_ID),
				BusinessEntityDefinitionId = reader.GetGuid(COL_BusinessEntityDefinitionID),
				BusinessEntityId = reader.GetString(COL_BusinessEntityID),
				MoreInfo = reader.GetString(COL_MoreInfo),
				PreviousMoreInfo = reader.GetString(COL_PreviousMoreInfo),

			};
		}
		#endregion

	}
}
