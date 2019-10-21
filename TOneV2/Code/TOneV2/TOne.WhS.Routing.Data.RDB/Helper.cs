using System.Collections.Generic;
using System.Linq;
using Vanrise.Data.RDB;

namespace TOne.WhS.Routing.Data.RDB
{
    public static class Helper
    {
        public static Dictionary<string, RDBTableColumnDefinition> GetRDBTableColumnDefinitions(Dictionary<string, RoutingTableColumnDefinition> routingTableColumnDefinitionDict)
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();

            foreach (var kvp_routingTableColumnDefinition in routingTableColumnDefinitionDict)
            {
                string columnName = kvp_routingTableColumnDefinition.Key;
                var routingTableColumnDefinition = kvp_routingTableColumnDefinition.Value;

                RDBTableColumnDefinition rdbTableColumnDefinition = new RDBTableColumnDefinition()
                {
                    DataType = routingTableColumnDefinition.RDBDataType,
                    Size = routingTableColumnDefinition.Size,
                    Precision = routingTableColumnDefinition.Precision
                };
                columns.Add(columnName, rdbTableColumnDefinition);
            }

            return columns;
        }

        public static void AddRoutingTableColumns(RDBCreateTableQuery createTableQuery, Dictionary<string, RoutingTableColumnDefinition> routingTableColumnDefinitionDict)
        {
            foreach (var routingTableColumnDefinition in routingTableColumnDefinitionDict)
            {
                string columnName = routingTableColumnDefinition.Key;
                var colDefinition = routingTableColumnDefinition.Value;

                createTableQuery.AddColumn(columnName, columnName, colDefinition.RDBDataType, colDefinition.Size, colDefinition.Precision,
                    colDefinition.NotNullable, colDefinition.IsPrimaryKey, colDefinition.IsIdentity);
            }
        }

        public static void AddRoutingTempTableColumns(RDBTempTableQuery tempTableQuery, Dictionary<string, RoutingTableColumnDefinition> routingTableColumnDefinitionDict, List<string> primaryKeyColumns)
        {
            foreach (var routingTableColumnDefinition in routingTableColumnDefinitionDict)
            {
                string columnName = routingTableColumnDefinition.Key;
                var colDefinition = routingTableColumnDefinition.Value;

                bool isPrimaryKey = (primaryKeyColumns != null && primaryKeyColumns.Contains(columnName));

                tempTableQuery.AddColumn(columnName, colDefinition.RDBDataType, colDefinition.Size, colDefinition.Precision, isPrimaryKey);
            }
        }
    }
}