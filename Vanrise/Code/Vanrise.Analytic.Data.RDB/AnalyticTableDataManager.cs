using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.Analytic.Data.RDB
{
    public class AnalyticTableDataManager : IAnalyticTableDataManager
    {

        public static string TABLE_NAME = "Analytic_AnalyticTable";
        static string TABLE_ALIAS = "vrAnalyticTable";

        public const string COL_ID = "ID";
        public const string COL_DevProjectID = "DevProjectID";
        public const string COL_Name = "Name";
        public const string COL_Settings = "Settings";
        public const string COL_CreatedTime = "CreatedTime";
        public const string COL_LastModifiedTime = "LastModifiedTime";
        public const string COL_MeasureStyles = "MeasureStyles";
        public const string COL_PermanentFilter = "PermanentFilter";

        static AnalyticTableDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_DevProjectID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_MeasureStyles, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_PermanentFilter, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Analytic",
                DBTableName = "AnalyticTable",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Analytic", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        AnalyticTable AnalyticTableMapper(IRDBDataReader reader)
        {
            AnalyticTable analyticTable = new AnalyticTable
            {
                AnalyticTableId = reader.GetGuid(COL_ID),
                DevProjectId = reader.GetNullableGuid(COL_DevProjectID),
                Name = reader.GetString(COL_Name),
                Settings = Vanrise.Common.Serializer.Deserialize<AnalyticTableSettings>(reader.GetString(COL_Settings)),
                PermanentFilter = Vanrise.Common.Serializer.Deserialize<AnalyticTablePermanentFilter>(reader.GetString(COL_PermanentFilter))
            };
            return analyticTable;
        }
        #endregion

        #region IAnalyticTableDataManager

        public bool AddAnalyticTable(Analytic.Entities.AnalyticTable analyticTable)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(analyticTable.Name);

            insertQuery.Column(COL_ID).Value(analyticTable.AnalyticTableId);
            insertQuery.Column(COL_Name).Value(analyticTable.Name);
            if(analyticTable.DevProjectId.HasValue)
            insertQuery.Column(COL_DevProjectID).Value(analyticTable.DevProjectId.Value);

            if (analyticTable.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(analyticTable.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreAnalyticTableUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<Analytic.Entities.AnalyticTable> GetAnalyticTables()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(AnalyticTableMapper);
        }

        public bool UpdateAnalyticTable(Analytic.Entities.AnalyticTable analyticTable)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(analyticTable.AnalyticTableId);
            notExistsCondition.EqualsCondition(COL_Name).Value(analyticTable.Name);

            updateQuery.Column(COL_Name).Value(analyticTable.Name);
            if (analyticTable.DevProjectId.HasValue)
                updateQuery.Column(COL_DevProjectID).Value(analyticTable.DevProjectId.Value);
            else
                updateQuery.Column(COL_DevProjectID).Null();
            if (analyticTable.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(analyticTable.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(analyticTable.AnalyticTableId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool SaveAnalyticTableMeasureStyles(AnalyticTableMeasureStyles measureStyles, Guid analyticTableId)
        {
            throw new NotImplementedException();
        }
        public bool SaveAnalyticTablePermanentFilter(AnalyticTablePermanentFilter permanentFilter, Guid analyticTableId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (permanentFilter!=null)
                updateQuery.Column(COL_PermanentFilter).Value(Vanrise.Common.Serializer.Serialize(permanentFilter));

            updateQuery.Where().EqualsCondition(COL_ID).Value(analyticTableId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
