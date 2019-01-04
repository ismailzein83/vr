using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.RDB
{
    class OrgChartDataManager : IOrgChartDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_OrgChart";
        static string TABLE_ALIAS = "orgChart";
        const string COL_Id = "Id";
        const string COL_Name = "Name";
        const string COL_Hierarchy = "Hierarchy";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static OrgChartDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Id, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 100 });
            columns.Add(COL_Hierarchy, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "OrgChart",
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
        OrgChart OrgChartMapper(IRDBDataReader reader)
        {
            return new OrgChart
            {
                OrgChartId = reader.GetInt(COL_Id),
                Name = reader.GetString(COL_Name),
                Hierarchy = Vanrise.Common.Serializer.Deserialize<List<Member>>(reader.GetString(COL_Hierarchy))
            };
        }
        #endregion
        #region IOrgChartDataManager
        public bool AddOrgChart(OrgChart orgChart, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.IfNotExists(TABLE_ALIAS).EqualsCondition(COL_Name).Value(orgChart.Name);
            insertQuery.Column(COL_Name).Value(orgChart.Name);
            if (orgChart.Hierarchy != null)
                insertQuery.Column(COL_Hierarchy).Value(Common.Serializer.Serialize(orgChart.Hierarchy));
            insertQuery.AddSelectGeneratedId();
            var id = queryContext.ExecuteScalar().NullableIntValue;
            if (id.HasValue)
                insertedId = id.Value;
            else
                insertedId = -1;
            return insertedId != -1;
        }

        public bool AreOrgChartsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool DeleteOrgChart(int orgChartId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_Id).Value(orgChartId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public IEnumerable<OrgChart> GetOrgCharts()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(OrgChartMapper);
        }

        public bool UpdateOrgChart(OrgChart orgChart)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(orgChart.Name);
            ifNotExists.NotEqualsCondition(COL_Id).Value(orgChart.OrgChartId);
            updateQuery.Column(COL_Name).Value(orgChart.Name);
            if (orgChart.Hierarchy != null)
                updateQuery.Column(COL_Hierarchy).Value(Common.Serializer.Serialize(orgChart.Hierarchy));
            else
                updateQuery.Column(COL_Hierarchy).Null();
            updateQuery.Where().EqualsCondition(COL_Id).Value(orgChart.OrgChartId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
