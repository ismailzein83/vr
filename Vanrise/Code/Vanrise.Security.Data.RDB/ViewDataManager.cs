using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.RDB
{
    public class ViewDataManager : IViewDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_View";
        static string TABLE_ALIAS = "view";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Title = "Title";
        const string COL_Url = "Url";
        const string COL_Module = "Module";
        const string COL_ActionNames = "ActionNames";
        const string COL_Audience = "Audience";
        const string COL_Content = "Content";
        const string COL_Settings = "Settings";
        const string COL_Type = "Type";
        const string COL_Rank = "Rank";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static ViewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Url, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Module, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_ActionNames, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_Audience, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Content, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Type, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Rank, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "View",
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
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion

        #region Mappers
        private View ViewMapper(IRDBDataReader reader)
        {
            var view = new View
            {
                ViewId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Title = reader.GetString(COL_Title),
                Url = reader.GetString(COL_Url),
                ActionNames = reader.GetString(COL_ActionNames),
                Audience = Common.Serializer.Deserialize<AudienceWrapper>(reader.GetString(COL_Audience)),
                ViewContent = Common.Serializer.Deserialize<ViewContent>(reader.GetString(COL_Content)),
                Type = reader.GetGuid(COL_Type)
            };
            var module = reader.GetNullableGuid(COL_Module);
            if (module.HasValue)
                view.ModuleId = module.Value;
            var rank = reader.GetNullableInt(COL_Rank);
            if (rank.HasValue)
                view.Rank = rank.Value;
            var settings = reader.GetString(COL_Settings);
            if (!string.IsNullOrEmpty(settings))
                view.Settings = Common.Serializer.Deserialize(settings) as ViewSettings;
            return view;
        }
        #endregion


        #region IViewDataManager
        public bool AddView(View view)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(view.Name);
            if (view.ModuleId.HasValue)
                ifNotExists.EqualsCondition(COL_Module).Value(view.ModuleId.Value);
            insertQuery.Column(COL_ID).Value(view.ViewId);
            insertQuery.Column(COL_Name).Value(view.Name);
            insertQuery.Column(COL_Title).Value(view.Title);
            insertQuery.Column(COL_Url).Value(view.Url);
            if (view.ModuleId.HasValue)
                insertQuery.Column(COL_Module).Value(view.ModuleId.Value);
            else
                insertQuery.Column(COL_Module).Null();
            if (view.Audience != null)
            {
                if ((view.Audience.Groups != null && view.Audience.Groups.Count > 0) || (view.Audience.Users != null && view.Audience.Users.Count > 0))
                    insertQuery.Column(COL_Audience).Value(Common.Serializer.Serialize(view.Audience, true));
                else
                    insertQuery.Column(COL_Audience).Null();
            }
            else
            {
                insertQuery.Column(COL_Audience).Null();
            }
            if (view.ViewContent != null)
            {
                if ((view.ViewContent.BodyContents != null && view.ViewContent.BodyContents.Count > 0) || (view.ViewContent.SummaryContents != null && view.ViewContent.SummaryContents.Count > 0))
                    insertQuery.Column(COL_Content).Value(Common.Serializer.Serialize(view.ViewContent, true));
                else
                    insertQuery.Column(COL_Content).Null();
            }
            else
            {
                insertQuery.Column(COL_Content).Null();
            }
            if (view.Settings != null)
                insertQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(view.Settings));
            else
                insertQuery.Column(COL_Settings).Null();
            insertQuery.Column(COL_Type).Value(view.Type);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreViewsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool DeleteView(Guid viewId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ID).Value(viewId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public void GenerateScript(List<View> views, Action<string, string> addEntityScript)
        {
            throw new NotImplementedException();
        }

        public List<View> GetViews()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereQuery = selectQuery.Where();
            var columnNotNullCondition = whereQuery.ConditionIfColumnNotNull(COL_IsDeleted, RDBConditionGroupOperator.OR);
            columnNotNullCondition.EqualsCondition(COL_IsDeleted).Value(false);
            var sort = selectQuery.Sort();
            sort.ByColumn(COL_Module, RDBSortDirection.ASC);
            sort.ByColumn(COL_Rank, RDBSortDirection.ASC);
            return queryContext.GetItems(ViewMapper);
        }

        public bool UpdateView(View view)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(view.Name);
            if (view.ModuleId.HasValue)
                ifNotExists.EqualsCondition(COL_Module).Value(view.ModuleId.Value);
            ifNotExists.NotEqualsCondition(COL_ID).Value(view.ViewId);
            updateQuery.Column(COL_Name).Value(view.Name);
            updateQuery.Column(COL_Url).Value(view.Url);
            if (view.ModuleId.HasValue)
                updateQuery.Column(COL_Module).Value(view.ModuleId.Value);
            else
                updateQuery.Column(COL_Module).Null();
            updateQuery.Column(COL_Title).Value(view.Title);
            if (view.Audience != null)
            {
                if ((view.Audience.Groups != null && view.Audience.Groups.Count > 0) || (view.Audience.Users != null && view.Audience.Users.Count > 0))
                    updateQuery.Column(COL_Audience).Value(Common.Serializer.Serialize(view.Audience, true));
                else
                    updateQuery.Column(COL_Audience).Null();
            }
            else
            {
                updateQuery.Column(COL_Audience).Null();
            }
            if (view.ViewContent != null)
            {
                if ((view.ViewContent.BodyContents != null && view.ViewContent.BodyContents.Count > 0) || (view.ViewContent.SummaryContents != null && view.ViewContent.SummaryContents.Count > 0))
                    updateQuery.Column(COL_Content).Value(Common.Serializer.Serialize(view.ViewContent, true));
                else
                    updateQuery.Column(COL_Content).Null();
            }
            else
            {
                updateQuery.Column(COL_Content).Null();
            }
            if (view.Settings != null)
                updateQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(view.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(view.ViewId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateViewRank(Guid viewId, Guid moduleId, int rank)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Rank).Value(rank);
            updateQuery.Column(COL_Module).Value(moduleId);
            updateQuery.Where().EqualsCondition(COL_ID).Value(viewId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
