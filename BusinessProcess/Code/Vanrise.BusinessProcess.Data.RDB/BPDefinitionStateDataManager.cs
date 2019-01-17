using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPDefinitionStateDataManager : IBPDefinitionStateDataManager
    {
        static string TABLE_NAME = "bp_BPDefinitionState";
        static string TABLE_ALIAS = "DefState";

        const string COL_DefinitionID = "DefinitionID";
        const string COL_ObjectKey = "ObjectKey";
        const string COL_ObjectValue = "ObjectValue";

        static BPDefinitionStateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_DefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_ObjectKey, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_ObjectValue, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPDefinitionState",
                Columns = columns
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess", "BusinessProcessDBConnStringKey", "BusinessProcessDBConnString");
        }


        public T GetDefinitionObjectState<T>(Guid definitionId, string objectKey)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_ObjectValue);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_DefinitionID).Value(definitionId);

            if (!string.IsNullOrEmpty(objectKey))
                whereContext.EqualsCondition(COL_ObjectKey).Value(objectKey);
            else
                whereContext.EqualsCondition(COL_ObjectKey).EmptyString();

            string objectVal = queryContext.ExecuteScalar().StringValue;
            return objectVal != null ? Serializer.Deserialize<T>(objectVal) : Activator.CreateInstance<T>();
        }

        public int InsertDefinitionObjectState(Guid definitionId, string objectKey, object objectValue)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_DefinitionID).Value(definitionId);

            if (!string.IsNullOrEmpty(objectKey))
                insertQuery.Column(COL_ObjectKey).Value(objectKey);
            else
                insertQuery.Column(COL_ObjectKey).EmptyString();

            if (objectValue != null)
                insertQuery.Column(COL_ObjectValue).Value(Serializer.Serialize(objectValue));

            return queryContext.ExecuteNonQuery();
        }

        public int UpdateDefinitionObjectState(Guid definitionId, string objectKey, object objectValue)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (objectValue != null)
                updateQuery.Column(COL_ObjectValue).Value(Serializer.Serialize(objectValue));
            else
                updateQuery.Column(COL_ObjectValue).Null();

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_DefinitionID).Value(definitionId);

            if (!string.IsNullOrEmpty(objectKey))
                whereContext.EqualsCondition(COL_ObjectKey).Value(objectKey);
            else
                whereContext.EqualsCondition(COL_ObjectKey).EmptyString();

            return queryContext.ExecuteNonQuery();
        }
    }
}
