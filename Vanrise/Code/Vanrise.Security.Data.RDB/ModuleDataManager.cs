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
    public class ModuleDataManager : IModuleDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_Module";
        static string TABLE_ALIAS = "module";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Url = "Url";
        const string COL_DefaultViewId = "DefaultViewId";
        const string COL_ParentId = "ParentId";
        const string COL_Icon = "Icon";
        const string COL_Rank = "Rank";
        const string COL_AllowDynamic = "AllowDynamic";
        const string COL_Settings = "Settings";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static ModuleDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Url, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_DefaultViewId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_ParentId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Icon, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_Rank, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_AllowDynamic, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "Module",
                Columns = columns,
                IdColumnName = COL_ID,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }


        #endregion

        #region Mappers
        Module ModuleMapper(IRDBDataReader reader)
        {
            var module =  new Entities.Module
            {
                ModuleId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Url = reader.GetString(COL_Url),
                DefaultViewId = reader.GetNullableGuid(COL_DefaultViewId),
                ParentId = reader.GetNullableGuid(COL_ParentId),
                Icon = reader.GetString(COL_Icon),
                AllowDynamic = true,
                Settings = Common.Serializer.Deserialize<ModuleSettings>(reader.GetString(COL_Settings))
            };
            var rank = reader.GetNullableInt(COL_Rank);
            if (rank.HasValue)
                module.Rank = rank.Value;
            return module;
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion
        #region IModuleDataManager
        public bool AddModule(Module moduleObject)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(moduleObject.Name);
            if (moduleObject.ParentId.HasValue)
                ifNotExists.EqualsCondition(COL_ParentId).Value(moduleObject.ParentId.Value);
            insertQuery.Column(COL_ID).Value(moduleObject.ModuleId);
            insertQuery.Column(COL_Name).Value(moduleObject.Name);
            if (moduleObject.ParentId.HasValue)
                insertQuery.Column(COL_ParentId).Value(moduleObject.ParentId.Value);
            if (moduleObject.DefaultViewId.HasValue)
                insertQuery.Column(COL_DefaultViewId).Value(moduleObject.DefaultViewId.Value);
            insertQuery.Column(COL_AllowDynamic).Value(moduleObject.AllowDynamic);
            if (moduleObject.Settings != null)
                insertQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(moduleObject.Settings));
            return queryContext.ExecuteNonQuery() > 0;

        }

        public bool AreModulesUpdated(ref object _updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref _updateHandle);
        }

        public List<Module> GetModules()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Rank, RDBSortDirection.ASC);
            return queryContext.GetItems(ModuleMapper);
        }

        public bool UpdateModule(Module moduleObject)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(moduleObject.ModuleId);
            ifNotExists.EqualsCondition(COL_Name).Value(moduleObject.Name);
            if (moduleObject.ParentId.HasValue)
                ifNotExists.EqualsCondition(COL_ParentId).Value(moduleObject.ParentId.Value);
            updateQuery.Column(COL_Name).Value(moduleObject.Name);
            if (moduleObject.ParentId.HasValue)
                updateQuery.Column(COL_ParentId).Value(moduleObject.ParentId.Value);
            else
                updateQuery.Column(COL_ParentId).Null();
            if (moduleObject.DefaultViewId.HasValue)
                updateQuery.Column(COL_DefaultViewId).Value(moduleObject.DefaultViewId.Value);
            else
                updateQuery.Column(COL_DefaultViewId).Null();
            updateQuery.Column(COL_AllowDynamic).Value(moduleObject.AllowDynamic);
            if (moduleObject.Settings != null)
                updateQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(moduleObject.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(moduleObject.ModuleId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateModuleRank(Guid moduleId, Guid? parentId, int rank)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Rank).Value(rank);
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
