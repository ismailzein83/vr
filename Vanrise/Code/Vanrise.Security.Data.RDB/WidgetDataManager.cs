using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.RDB
{
    public class WidgetDataManager : IWidgetsDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_Widget";
        static string TABLE_ALIAS = "widget";
        const string COL_Id = "Id";
        const string COL_WidgetDefinitionId = "WidgetDefinitionId";
        const string COL_Name = "Name";
        const string COL_Title = "Title";
        const string COL_Setting = "Setting";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static WidgetDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Id, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_WidgetDefinitionId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_Setting, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "Widget",
                Columns = columns,
                IdColumnName = COL_Id,
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
        private Widget WidgetMapper(IRDBDataReader reader)
        {
            Widget widget = new Widget
            {
                Id = reader.GetInt(COL_Id),
                Name = reader.GetString(COL_Name),
                Title = reader.GetString(COL_Title),
                WidgetDefinitionId = reader.GetInt(COL_WidgetDefinitionId),
                Setting = Common.Serializer.Deserialize<WidgetSetting>(reader.GetString(COL_Setting))
            };
            return widget;
        }
        #endregion

        #region IWidgetsDataManager
        public bool AddWidget(Widget widget, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.OR);
            ifNotExists.EqualsCondition(COL_Name).Value(widget.Name);
            if (widget.Setting != null)
                ifNotExists.EqualsCondition(COL_Setting).Value(Common.Serializer.Serialize(widget.Setting));
            insertQuery.Column(COL_WidgetDefinitionId).Value(widget.WidgetDefinitionId);
            insertQuery.Column(COL_Name).Value(widget.Name);
            insertQuery.Column(COL_Title).Value(widget.Title);
            if (widget.Setting != null)
                insertQuery.Column(COL_Setting).Value(Common.Serializer.Serialize(widget.Setting));
            insertQuery.AddSelectGeneratedId();
            var id = queryContext.ExecuteScalar().NullableIntValue;
            if (id.HasValue)
                insertedId = id.Value;
            else
                insertedId = -1;
            return insertedId != -1;
        }

        public bool AreAllWidgetsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public int CheckWidgetSetting(Widget widget)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereQuery = selectQuery.Where();
            if (widget.Setting != null)
                whereQuery.EqualsCondition(COL_Setting).Value(Common.Serializer.Serialize(widget.Setting));
            else
                whereQuery.NullCondition(COL_Setting);
            if (widget.Id != 0)
            {
                whereQuery.NotEqualsCondition(COL_Id).Value(widget.Id);
            }
            var widgetItem = queryContext.GetItem(WidgetMapper);
            return widgetItem != null ? 1 : 0;
        }

        public bool DeleteWidget(int widgetId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_Id).Value(widgetId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public List<Widget> GetAllWidgets()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(WidgetMapper);
        }

        public bool UpdateWidget(Widget widget)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(widget.Name);
            ifNotExists.NotEqualsCondition(COL_Id).Value(widget.Id);
            updateQuery.Column(COL_WidgetDefinitionId).Value(widget.WidgetDefinitionId);
            updateQuery.Column(COL_Name).Value(widget.Name);
            updateQuery.Column(COL_Title).Value(widget.Title);
            if (widget.Setting != null)
                updateQuery.Column(COL_Setting).Value(Common.Serializer.Serialize(widget.Setting));
            else
                updateQuery.Column(COL_Setting).Null();
            updateQuery.Where().EqualsCondition(COL_Id).Value(widget.Id);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
