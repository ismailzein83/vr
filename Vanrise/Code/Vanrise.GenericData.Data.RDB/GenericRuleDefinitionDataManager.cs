using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.RDB
{
    public class GenericRuleDefinitionDataManager : IGenericRuleDefinitionDataManager
    {
        #region RDB
        static string TABLE_NAME = "genericdata_GenericRuleDefinition";
        static string TABLE_ALIAS = "genericRuleDefinition";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Details = "Details";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static GenericRuleDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 900 });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "GenericRuleDefinition",
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
        GenericRuleDefinition GenericRuleDefinitionMapper(IRDBDataReader reader)
        {
            GenericRuleDefinition details = Vanrise.Common.Serializer.Deserialize<GenericRuleDefinition>(reader.GetString(COL_Details));
            return new GenericRuleDefinition()
            {
                GenericRuleDefinitionId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Title = details.Title,
                Objects = details.Objects,
                CriteriaDefinition = details.CriteriaDefinition,
                SettingsDefinition = details.SettingsDefinition,
                Security = details.Security
            };
        }

        #endregion

        #region IGenericRuleDefinitionDataManager

        public IEnumerable<GenericRuleDefinition> GetGenericRuleDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(GenericRuleDefinitionMapper);
        }

        public bool AreGenericRuleDefinitionsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool AddGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(genericRuleDefinition.Name);
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(genericRuleDefinition.GenericRuleDefinitionId);
            insertQuery.Column(COL_Name).Value(genericRuleDefinition.Name);
            insertQuery.Column(COL_Details).Value(Common.Serializer.Serialize(genericRuleDefinition));
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(genericRuleDefinition.GenericRuleDefinitionId);
            ifNotExists.EqualsCondition(COL_Name).Value(genericRuleDefinition.Name);
            updateQuery.Column(COL_Name).Value(genericRuleDefinition.Name);
            updateQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(genericRuleDefinition));
            updateQuery.Where().EqualsCondition(COL_ID).Value(genericRuleDefinition.GenericRuleDefinitionId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public void GenerateScript(List<GenericRuleDefinition> ruleDefinitions, Action<string, string> addEntityScript)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
