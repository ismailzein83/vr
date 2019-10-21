using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class ModifiedCustomerRouteDataManager : RoutingDataManager
    {
        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "ModifiedCustomerRoute";
        internal static string TABLE_NAME = "dbo_ModifiedCustomerRoute";
        internal static string TABLE_ALIAS = "mcr";

        internal const string COL_CustomerId = "CustomerId";
        internal const string COL_Code = "Code";
        internal const string COL_VersionNumber = "VersionNumber";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_ModifiedCustomerRouteColumnDefinitions;

        static ModifiedCustomerRouteDataManager()
        {
            s_ModifiedCustomerRouteColumnDefinitions = BuildModifiedCustomerRouteColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_ModifiedCustomerRouteColumnDefinitions);

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = DBTABLE_SCHEMA,
                DBTableName = DBTABLE_NAME,
                Columns = columns
                //IdColumnName = COL_QualityConfigurationId
            });
        }

        public void AddJoinModifiedCustomerRouteByCustomerAndCode(RDBJoinContext joinContext, RDBJoinType joinType, string modifiedCustomerRouteTableAlias, string originalTableAlias,
            string originalTableCustomerCol, string originalTableCodeCol, bool withNoLock)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, TABLE_ALIAS);
            joinStatement.JoinType(joinType);

            if (withNoLock)
                joinStatement.WithNoLock();

            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(modifiedCustomerRouteTableAlias, COL_CustomerId, originalTableAlias, originalTableCustomerCol);
            joinCondition.EqualsCondition(modifiedCustomerRouteTableAlias, COL_Code, originalTableAlias, originalTableCodeCol);
        }

        private static Dictionary<string, RoutingTableColumnDefinition> BuildModifiedCustomerRouteColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_CustomerId, new RoutingTableColumnDefinition(COL_CustomerId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_Code, new RoutingTableColumnDefinition(COL_Code, RDBDataType.Varchar, 20, 0,  true));
            columnDefinitions.Add(COL_VersionNumber, new RoutingTableColumnDefinition(COL_VersionNumber, RDBDataType.Int, true));
            return columnDefinitions;
        }
    }
}