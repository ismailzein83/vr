using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;
using Vanrise.Entities;
namespace Vanrise.Security.Data.RDB
{
    public class BusinessEntityModuleDataManager : IBusinessEntityModuleDataManager
    {
        #region RDB

        static string TABLE_NAME = "sec_BusinessEntityModule";
        static string TABLE_ALIAS = "module";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_ParentId = "ParentId";
        const string COL_BreakInheritance = "BreakInheritance";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static BusinessEntityModuleDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_ParentId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_BreakInheritance, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "BusinessEntityModule",
                Columns = columns,
                IdColumnName = COL_ID,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }


        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion

        #region Mappers
        BusinessEntityModule ModuleMapper(IRDBDataReader reader)
        {
            BusinessEntityModule module = new Entities.BusinessEntityModule
            {
                ModuleId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                ParentId = reader.GetNullableGuid(COL_ParentId),
                BreakInheritance = reader.GetBoolean(COL_BreakInheritance),
                PermissionOptions = new List<string>() { "View", "Add", "Edit", "Delete", "Full Control" }
            };
            return module;
        }

        #endregion

        #region IBusinessEntityModuleDataManager

        public bool AddBusinessEntityModule(BusinessEntityModule moduleObject)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.IfNotExists(TABLE_ALIAS).EqualsCondition(COL_Name).Value(moduleObject.Name);
            insertQuery.Column(COL_ID).Value(moduleObject.ModuleId);
            insertQuery.Column(COL_Name).Value(moduleObject.Name);
            if (moduleObject.ParentId.HasValue)
                insertQuery.Column(COL_ParentId).Value(moduleObject.ParentId.Value);
            insertQuery.Column(COL_BreakInheritance).Value(moduleObject.BreakInheritance);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreBusinessEntityModulesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<BusinessEntityModule> GetModules()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(ModuleMapper);
        }

        public bool ToggleBreakInheritance(Guid entityId)
        {
            var selectQueryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().EqualsCondition(COL_ID).Value(entityId);
            var item = selectQueryContext.GetItem(ModuleMapper);
            if (item != null)
            {
                var updateQueryContext = new RDBQueryContext(GetDataProvider());
                var updateQuery = updateQueryContext.AddUpdateQuery();
                updateQuery.FromTable(TABLE_NAME);
                updateQuery.Column(COL_BreakInheritance).Value(!item.BreakInheritance);
                updateQuery.Where().EqualsCondition(COL_ID).Value(entityId);
                return updateQueryContext.ExecuteNonQuery() > 0;
            }
            return false;
        }

        public bool UpdateBusinessEntityModule(BusinessEntityModule moduleObject)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(moduleObject.Name);
            ifNotExists.NotEqualsCondition(COL_ID).Value(moduleObject.ModuleId);
            updateQuery.Column(COL_Name).Value(moduleObject.Name);
            if (moduleObject.ParentId.HasValue)
                updateQuery.Column(COL_ParentId).Value(moduleObject.ParentId.Value);
            else
                updateQuery.Column(COL_ParentId).Null();
            updateQuery.Column(COL_BreakInheritance).Value(moduleObject.BreakInheritance);
            updateQuery.Where().EqualsCondition(COL_ID).Value(moduleObject.ModuleId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateBusinessEntityModuleRank(Guid moduleId, Guid? parentId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (parentId.HasValue)
                updateQuery.Column(COL_ParentId).Value(parentId.Value);
            else
                updateQuery.Column(COL_ParentId).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(moduleId);
            return queryContext.ExecuteNonQuery() > 0;

        }
        #endregion
    }
}
