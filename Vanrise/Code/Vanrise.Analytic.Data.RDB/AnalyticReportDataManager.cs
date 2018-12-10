using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.Analytic.Data.RDB
{
    public class AnalyticReportDataManager : IAnalyticReportDataManager
    {

        static string TABLE_NAME = "VR_Analytic_AnalyticReport";
        const string COL_ID = "ID";
        const string COL_UserID = "UserID";
        const string COL_Name = "Name";
        const string COL_AccessType = "AccessType";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        static AnalyticReportDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_AccessType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Analytic",
                DBTableName = "AnalyticReport",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Analytic", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        AnalyticReport AnalyticReportMapper(IRDBDataReader reader)
        {
            AnalyticReport analyticTable = new AnalyticReport
            {
                AnalyticReportId = reader.GetGuid(COL_ID),
                AccessType= (AccessType)reader.GetInt(COL_AccessType),
                UserID=reader.GetInt(COL_UserID),
                Name = reader.GetString(COL_Name),
                Settings = Vanrise.Common.Serializer.Deserialize<AnalyticReportSettings>(reader.GetString(COL_Settings)),
            };
            return analyticTable;
        }
        #endregion

        #region IAnalyticReportDataManager

        public bool AddAnalyticReport(AnalyticReport analyticReport)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var ifNotExists = insertQuery.IfNotExists("vrAnalyticReport");
            ifNotExists.EqualsCondition(COL_Name).Value(analyticReport.Name);

            insertQuery.Column(COL_ID).Value(analyticReport.AnalyticReportId);
            insertQuery.Column(COL_Name).Value(analyticReport.Name);
            insertQuery.Column(COL_UserID).Value(analyticReport.UserID);
            insertQuery.Column(COL_AccessType).Value((int)analyticReport.AccessType);
            if (analyticReport.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(analyticReport.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreAnalyticReportUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public List<AnalyticReport> GetAnalyticReports()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "vrAnalyticReport", null, true);
            selectQuery.SelectColumns().AllTableColumns("vrAnalyticReport");
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(AnalyticReportMapper);
        }

        public bool UpdateAnalyticReport(AnalyticReport analyticReport)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists("vrAnalyticReport");
            notExistsCondition.NotEqualsCondition(COL_ID).Value(analyticReport.AnalyticReportId);
            notExistsCondition.EqualsCondition(COL_Name).Value(analyticReport.Name);

            updateQuery.Column(COL_Name).Value(analyticReport.Name);
            updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(analyticReport.Settings));
            updateQuery.Column(COL_UserID).Value(analyticReport.UserID);
            updateQuery.Column(COL_AccessType).Value((int)analyticReport.AccessType);

            updateQuery.Where().EqualsCondition(COL_ID).Value(analyticReport.AnalyticReportId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
