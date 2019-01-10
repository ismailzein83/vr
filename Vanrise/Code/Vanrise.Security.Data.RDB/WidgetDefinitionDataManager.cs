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
    public class WidgetDefinitionDataManager : IWidgetDefinitionDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_WidgetDefinition";
        static string TABLE_ALIAS = "widgetDefinition";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_DirectiveName = "DirectiveName";
        const string COL_Setting = "Setting";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static WidgetDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_DirectiveName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_Setting, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "WidgetDefinition",
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
        private WidgetDefinition WidgetDefinitionMapper(IRDBDataReader reader)
        {
            WidgetDefinition instance = new WidgetDefinition
            {
                ID = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                DirectiveName = reader.GetString(COL_DirectiveName),
                Setting = Common.Serializer.Deserialize<WidgetDefinitionSetting>(reader.GetString(COL_Setting))
            };
            return instance;
        }
        #endregion

        #region IWidgetDefinitionDataManager
        public bool AreWidgetDefinitionsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<WidgetDefinition> GetWidgetsDefinition()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(WidgetDefinitionMapper);
        }
        #endregion
    }
}
