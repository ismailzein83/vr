using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.Security.Data.RDB
{
    public class OrgChartLinkedEntityDataManager : IOrgChartLinkedEntityDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_OrgChartLinkedEntity";
        static string TABLE_ALIAS = "entity";
        const string COL_OrgChartID = "OrgChartID";
        const string COL_LinkedEntityIdentifier = "LinkedEntityIdentifier";


        static OrgChartLinkedEntityDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_OrgChartID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LinkedEntityIdentifier, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 850 });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "OrgChartLinkedEntity",
                Columns = columns
            });
        }

        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion

        #region IOrgChartLinkedEntityDataManager
        public int? GetLinkedOrgChartId(string linkedEntityIdentifier)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_OrgChartID);
            selectQuery.Where().EqualsCondition(COL_LinkedEntityIdentifier).Value(linkedEntityIdentifier);
            return queryContext.ExecuteScalar().NullableIntValue;
        }

        public bool InsertOrUpdate(int orgChartId, string linkedEntityIdentifier)
        {
            var selectQueryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().Column(COL_LinkedEntityIdentifier);
            selectQuery.Where().EqualsCondition(COL_LinkedEntityIdentifier).Value(linkedEntityIdentifier);
            var identifier = selectQueryContext.ExecuteScalar().StringValue;
            if (identifier!=null)
            {
                var updateQueryContext = new RDBQueryContext(GetDataProvider());
                var updateQuery = updateQueryContext.AddUpdateQuery();
                updateQuery.FromTable(TABLE_NAME);
                updateQuery.Column(COL_OrgChartID).Value(orgChartId);
                updateQuery.Where().EqualsCondition(COL_LinkedEntityIdentifier).Value(linkedEntityIdentifier);
                return updateQueryContext.ExecuteNonQuery() > 0;
            }
            else
            {
                var insertQueryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = insertQueryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.Column(COL_OrgChartID).Value(orgChartId);
                insertQuery.Column(COL_LinkedEntityIdentifier).Value(linkedEntityIdentifier);
                return insertQueryContext.ExecuteNonQuery() > 0;
            }
        }
        #endregion
    }
}
