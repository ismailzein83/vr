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
    public class BusinessEntityDefinitionDataManager : IBusinessEntityDefinitionDataManager
    {
        #region RDB

        static string TABLE_NAME = "genericdata_BusinessEntityDefinition";
        static string TABLE_ALIAS = "beDefinition";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Title = "Title";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static BusinessEntityDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 900 });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "BusinessEntityDefinition",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        #endregion
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData_BusinessEntityDefinition", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #region PrivateMethods
        BusinessEntityDefinition BusinessEntityDefinitionMapper(IRDBDataReader reader)
        {
            return new BusinessEntityDefinition()
            {
                BusinessEntityDefinitionId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Title = reader.GetString(COL_Title),
                Settings = Vanrise.Common.Serializer.Deserialize<BusinessEntityDefinitionSettings>(reader.GetString(COL_Settings))
            };
        }

        #endregion

        #region Mappers

        #endregion
        #region IBusinessEntityDefinitionDataManager
        public bool AddBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(businessEntityDefinition.Name);
            insertQuery.Column(COL_ID).Value(businessEntityDefinition.BusinessEntityDefinitionId);
            insertQuery.Column(COL_Name).Value(businessEntityDefinition.Name);
            insertQuery.Column(COL_Title).Value(businessEntityDefinition.Title);
            if (businessEntityDefinition.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(businessEntityDefinition.Settings));
            else
                insertQuery.Column(COL_Settings).Null();
            return queryContext.ExecuteNonQuery() > 0;

        }

        public bool AreGenericRuleDefinitionsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public void GenerateScript(List<BusinessEntityDefinition> beDefinitions, Action<string, string> addEntityScript)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BusinessEntityDefinition> GetBusinessEntityDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems<BusinessEntityDefinition>(BusinessEntityDefinitionMapper);
        }

        public bool UpdateBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(businessEntityDefinition.Name);
            ifNotExists.NotEqualsCondition(COL_ID).Value(businessEntityDefinition.BusinessEntityDefinitionId);
            updateQuery.Column(COL_Name).Value(businessEntityDefinition.Name);
            updateQuery.Column(COL_Title).Value(businessEntityDefinition.Title);
            if (businessEntityDefinition.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(businessEntityDefinition.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(businessEntityDefinition.BusinessEntityDefinitionId);
            return queryContext.ExecuteNonQuery() > 0;

        }
        #endregion
    }
}
