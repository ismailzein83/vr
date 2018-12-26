using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.RDB
{
    public class BELookupRuleDefinitionDataManager : IBELookupRuleDefinitionDataManager
    {
        #region RDB
        static string TABLE_NAME = "genericdata_BELookupRuleDefinition";
        static string TABLE_ALIAS = "beLookupRuleDefinition";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Details = "Details";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static BELookupRuleDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 450 });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "BELookupRuleDefinition",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region Mappers
        BELookupRuleDefinition BELookupRuleDefinitionMapper(IRDBDataReader reader)
        {
            var beLookupRuleDefinition = Vanrise.Common.Serializer.Deserialize<BELookupRuleDefinition>(reader.GetString(COL_Details));
            beLookupRuleDefinition.BELookupRuleDefinitionId = reader.GetGuid(COL_ID);
            return beLookupRuleDefinition;
        }
        #endregion
        #region IBELookupRuleDefinitionDataManager
        public IEnumerable<BELookupRuleDefinition> GetBELookupRuleDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems<BELookupRuleDefinition>(BELookupRuleDefinitionMapper);
        }

        public bool AreBELookupRuleDefinitionsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool InsertBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
            ifNotExists.EqualsCondition(COL_Name).Value(beLookupRuleDefinition.Name);

            insertQuery.Column(COL_ID).Value(beLookupRuleDefinition.BELookupRuleDefinitionId);
            insertQuery.Column(COL_Name).Value(beLookupRuleDefinition.Name);
            insertQuery.Column(COL_Details).Value(Serializer.Serialize(beLookupRuleDefinition, true));
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
            ifNotExists.EqualsCondition(COL_Name).Value(beLookupRuleDefinition.Name);
            ifNotExists.NotEqualsCondition(COL_ID).Value(beLookupRuleDefinition.BELookupRuleDefinitionId);

            updateQuery.Column(COL_Name).Value(beLookupRuleDefinition.Name);
            updateQuery.Column(COL_Details).Value(Serializer.Serialize(beLookupRuleDefinition, true));

            updateQuery.Where().EqualsCondition(COL_ID).Value(beLookupRuleDefinition.BELookupRuleDefinitionId);

            return queryContext.ExecuteNonQuery() > 0;

        }
        #endregion
    }
}
