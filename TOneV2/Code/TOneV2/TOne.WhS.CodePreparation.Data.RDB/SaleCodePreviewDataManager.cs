using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.RDB;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class SaleCodePreviewDataManager : ISaleCodePreviewDataManager
    {
        #region RDB

        static string TABLE_NAME = "TOneWhs_CP_SaleCode_Preview";
        static string TABLE_ALIAS = "saleCodePrev";
        internal const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_Code = "Code";
        internal const string COL_ChangeType = "ChangeType";
        internal const string COL_RecentZoneName = "RecentZoneName";
        internal const string COL_ZoneName = "ZoneName";
        const string COL_BED = "BED";
        const string COL_EED = "EED";


        static SaleCodePreviewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_ChangeType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RecentZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_ZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhs_CP",
                DBTableName = "SaleCode_Preview",
                Columns = columns
            });
        }


        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("WhS_CodePrep", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion
        #region Mappers
        private CodePreview CodePreviewMapper(IRDBDataReader reader)
        {
            CodePreview codePreview = new CodePreview
            {
                Code = reader.GetString(COL_Code),
                ChangeType = (CodeChangeType)reader.GetInt(COL_ChangeType),
                RecentZoneName = reader.GetString(COL_RecentZoneName),
                ZoneName = reader.GetString(COL_ZoneName),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
            return codePreview;
        }
        #endregion

        #region ISaleCodePreviewDataManager
        readonly string[] _columns = { COL_ProcessInstanceID, COL_Code, COL_ChangeType, COL_RecentZoneName, COL_ZoneName, COL_BED, COL_EED };

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public void ApplyPreviewCodesToDB(object preparedCodes)
        {
            preparedCodes.CastWithValidate<RDBBulkInsertQueryContext>("preparedCodes").Apply();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public IEnumerable<CodePreview> GetFilteredCodePreview(SPLPreviewQuery query)
        {
            var saleZonePreviewDataManager = new SaleZonePreviewDataManager();
            var saleZonePreviewAlias = "saleZonePrev";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_Code, COL_ChangeType, COL_RecentZoneName, COL_ZoneName, COL_BED, COL_EED);
            var joinContext = selectQuery.Join();
            saleZonePreviewDataManager.SetJoinContext(joinContext, saleZonePreviewAlias, TABLE_ALIAS ,COL_ZoneName, COL_ProcessInstanceID);
            var where = selectQuery.Where();
            if (query.ZoneName != null)
            {
                var subcondition = where.ChildConditionGroup(RDBConditionGroupOperator.OR);
                subcondition.EqualsCondition(COL_ZoneName).Value(query.ZoneName);
                subcondition.EqualsCondition(COL_RecentZoneName).Value(query.ZoneName);
            }
            if (!query.OnlyModified)
                where.NotEqualsCondition(COL_ChangeType).Value((int)CodeChangeType.NotChanged);
            where.EqualsCondition(saleZonePreviewAlias, SaleZonePreviewDataManager.COL_ProcessInstanceID).Value(query.ProcessInstanceId);
            return queryContext.GetItems(CodePreviewMapper);
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', _columns);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(CodePreview record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(_processInstanceID);
            recordContext.Value(record.Code);
            recordContext.Value((int)record.ChangeType);
            recordContext.Value(record.RecentZoneName);
            recordContext.Value(record.ZoneName);
            recordContext.Value(record.BED);
            if (record.EED.HasValue)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Null();
        }
        #endregion

        #region CodePreparation
        public void DeleteRecords(RDBDeleteQuery deleteQuery, long processInstanceId)
        {
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
        }

        public void SetJoinContext(RDBJoinContext joinContext, string joinedTableAlias, string originalTableAlias, string zoneNameColumn)
        {
            var joinCondition = joinContext.Join(TABLE_NAME, joinedTableAlias).On(RDBConditionGroupOperator.OR);
            joinCondition.EqualsCondition(joinedTableAlias, COL_ZoneName, originalTableAlias, zoneNameColumn);
            joinCondition.EqualsCondition(joinedTableAlias, COL_RecentZoneName, originalTableAlias, zoneNameColumn);
        }
        #endregion
    }
}
