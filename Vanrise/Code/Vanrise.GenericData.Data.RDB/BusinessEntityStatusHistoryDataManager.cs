using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.RDB
{
    public class BusinessEntityStatusHistoryDataManager : IBusinessEntityStatusHistoryDataManager
    {
        #region RDB

        static string TABLE_NAME = "genericdata_BusinessEntityStatusHistory";
        static string TABLE_ALIAS = "statusHistory";
        const string COL_ID = "ID";
        const string COL_BusinessEntityDefinitionID = "BusinessEntityDefinitionID";
        const string COL_BusinessEntityID = "BusinessEntityID";
        const string COL_FieldName = "FieldName";
        const string COL_StatusID = "StatusID";
        const string COL_PreviousStatusID = "PreviousStatusID";
        const string COL_StatusChangedDate = "StatusChangedDate";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_CreatedTime = "CreatedTime";


        static BusinessEntityStatusHistoryDataManager()
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

        #region Mappers
        BusinessEntityStatusHistory BusinessEntityStatusHistoryMapper(IRDBDataReader reader)
        {
            return new BusinessEntityStatusHistory
            {
                StatusId = reader.GetGuid(COL_StatusID),
                StatusChangedDate = reader.GetDateTime(COL_StatusChangedDate),
                FieldName =reader.GetString(COL_FieldName),
                IsDeleted = reader.GetBoolean(COL_IsDeleted),
                PreviousStatusId = reader.GetNullableGuid(COL_PreviousStatusID),
                BusinessEntityStatusHistoryId = reader.GetLong(COL_ID),
                BusinessEntityDefinitionId = reader.GetGuid(COL_BusinessEntityDefinitionID),
                BusinessEntityId = reader.GetString(COL_BusinessEntityID),
            };
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion
        public BusinessEntityStatusHistory GetLastBusinessEntityStatusHistory(Guid businessEntityDefinitionId, string businessEntityId, string fieldName)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1 , false);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var subCondition = selectQuery.Where().ChildConditionGroup(RDBConditionGroupOperator.OR);
            subCondition.NullCondition(COL_IsDeleted);
            subCondition.EqualsCondition(COL_IsDeleted).Value(false);
            selectQuery.Where().EqualsCondition(COL_BusinessEntityDefinitionID).Value(businessEntityDefinitionId);
            selectQuery.Where().EqualsCondition(COL_BusinessEntityID).Value(businessEntityId);
            selectQuery.Where().EqualsCondition(COL_FieldName).Value(fieldName);
            selectQuery.Sort().ByColumn(COL_StatusChangedDate, RDBSortDirection.DESC);
            return queryContext.GetItem<BusinessEntityStatusHistory>(BusinessEntityStatusHistoryMapper);
        }

        public bool Insert(Guid businessEntityDefinitionId, string businessEntityId, string fieldName, Guid statusId, Guid? previousStatusId)
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
            else
                insertQuery.Column(COL_PreviousStatusID).Null();
            insertQuery.Column(COL_StatusChangedDate).DateNow();
            insertQuery.Column(COL_IsDeleted).Value(false);
            return queryContext.ExecuteNonQuery() > 0;
        }
    }
}
