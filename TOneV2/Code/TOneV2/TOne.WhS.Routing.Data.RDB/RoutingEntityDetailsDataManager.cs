using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class RoutingEntityDetailsDataManager : RoutingDataManager, IRoutingEntityDetailsDataManager
    {
        #region Fields/Ctor

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "RoutingEntityDetails";
        private static string TABLE_NAME = "dbo_RoutingEntityDetails";
        private static string TABLE_ALIAS = "red";

        private const string COL_Type = "Type";
        private const string COL_Info = "Info";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_RoutingEntityDetailsColumnDefinitions;

        static RoutingEntityDetailsDataManager()
        {
            s_RoutingEntityDetailsColumnDefinitions = BuildRoutingEntityDetailsColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_RoutingEntityDetailsColumnDefinitions);

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = DBTABLE_SCHEMA,
                DBTableName = DBTABLE_NAME,
                Columns = columns,
                //IdColumnName = COL_ID
            });
        }

        #endregion

        #region Public Methods

        public RoutingEntityDetails GetRoutingEntityDetails(RoutingEntityType routingEntityType)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, false);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_Type).Value((int)routingEntityType);

            return queryContext.GetItem(PartialRouteInfoMapper);
        }

        public void ApplyRoutingEntityDetails(RoutingEntityDetails routingEntityDetails)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RoutingEntityDetails dbRoutingEntityDetails = this.GetRoutingEntityDetails(routingEntityDetails.RoutingEntityType);
            if (dbRoutingEntityDetails != null)
            {
                RDBInsertQuery insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.Column(COL_Type).Value((int)routingEntityDetails.RoutingEntityType);
                insertQuery.Column(COL_Info).Value(Vanrise.Common.Serializer.Serialize(routingEntityDetails.RoutingEntityInfo));
            }
            else
            {
                RDBUpdateQuery updateQuery = queryContext.AddUpdateQuery();
                updateQuery.FromTable(TABLE_NAME);
                updateQuery.Column(COL_Info).Value(Vanrise.Common.Serializer.Serialize(routingEntityDetails.RoutingEntityInfo));

                var whereContext = updateQuery.Where();
                whereContext.EqualsCondition(COL_Type).Value((int)routingEntityDetails.RoutingEntityType);
            }

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #region Private Methods

        private RoutingEntityDetails PartialRouteInfoMapper(IRDBDataReader reader)
        {
            var info = reader.GetString("Info");

            return new RoutingEntityDetails()
            {
                RoutingEntityType = (RoutingEntityType)reader.GetInt("Type"),
                RoutingEntityInfo = !string.IsNullOrEmpty(info) ? Vanrise.Common.Serializer.Deserialize<RoutingEntityInfo>(info) : null
            };
        }


        private static Dictionary<string, RoutingTableColumnDefinition> BuildRoutingEntityDetailsColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_Type, new RoutingTableColumnDefinition(COL_Type, RDBDataType.Int, true));
            columnDefinitions.Add(COL_Info, new RoutingTableColumnDefinition(COL_Info, RDBDataType.NVarchar, true));
            return columnDefinitions;
        }

        #endregion
    }
}