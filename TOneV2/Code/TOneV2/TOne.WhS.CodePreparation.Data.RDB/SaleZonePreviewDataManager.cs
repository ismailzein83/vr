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
    public class SaleZonePreviewDataManager : ISaleZonePreviewDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhs_CP_SaleZone_Preview";
        static string TABLE_ALIAS = "saleZonePrev";
        internal const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_CountryID = "CountryID";
        internal const string COL_ZoneName = "ZoneName";
        const string COL_RecentZoneName = "RecentZoneName";
        const string COL_ZoneChangeType = "ZoneChangeType";
        const string COL_ZoneBED = "ZoneBED";
        const string COL_ZoneEED = "ZoneEED";


        static SaleZonePreviewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_RecentZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_ZoneChangeType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneBED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ZoneEED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhs_CP",
                DBTableName = "SaleZone_Preview",
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
        private ZonePreview ZonePreviewMapper(IRDBDataReader reader)
        {
            ZonePreview zonePreview = new ZonePreview
            {
                ZoneName = reader.GetString(COL_ZoneName),
                RecentZoneName = reader.GetString(COL_RecentZoneName),
                ChangeTypeZone = (ZoneChangeType)reader.GetInt(COL_ZoneChangeType),
                ZoneBED = reader.GetDateTime(COL_ZoneBED),
                ZoneEED = reader.GetNullableDateTime(COL_ZoneEED),
                NewCodes = reader.GetInt("NewCodes"),
                DeletedCodes = reader.GetInt("DeletedCodes"),
                CodesMovedFrom = reader.GetInt("CodesMovedFrom"),
                CodesMovedTo = reader.GetInt("CodesMovedTo")

            };
            return zonePreview;
        }

        #endregion

        #region ISaleZonePreviewDataManager
        readonly string[] _columns = { COL_ProcessInstanceID, COL_CountryID, COL_ZoneName, COL_RecentZoneName, COL_ZoneChangeType, COL_ZoneBED, COL_ZoneEED };
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;
        public void ApplyPreviewZonesToDB(object preparedZones)
        {
            preparedZones.CastWithValidate<RDBBulkInsertQueryContext>("preparedZones").Apply();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public IEnumerable<ZonePreview> GetFilteredZonePreview(SPLPreviewQuery query)
        {
            var saleCodePreviewDataManager = new SaleCodePreviewDataManager();
            string saleCodePreviewAlias = "saleCodePrev";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ZoneName, COL_RecentZoneName, COL_ZoneChangeType, COL_ZoneBED, COL_ZoneEED);
            var aggregates = selectQuery.SelectAggregates();

            var newCodesExpression = aggregates.ExpressionAggregate(RDBNonCountAggregateType.SUM, "NewCodes");
            var newCodesCase = newCodesExpression.CaseExpression();
            var newCodesCondition = newCodesCase.AddCase();
            newCodesCondition.When().EqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ChangeType).Value((int)CodeChangeType.New);
            newCodesCondition.Then().Value(1);
            newCodesCase.Else().Value(0);

            var deletedCodesExpression = aggregates.ExpressionAggregate(RDBNonCountAggregateType.SUM, "DeletedCodes");
            var deleteCodesCase = deletedCodesExpression.CaseExpression();
            var deleteCodesCondition = deleteCodesCase.AddCase();
            deleteCodesCondition.When().EqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ChangeType).Value((int)CodeChangeType.Deleted);
            deleteCodesCondition.Then().Value(1);
            deleteCodesCase.Else().Value(0);

            var codesMovedToExpression = aggregates.ExpressionAggregate(RDBNonCountAggregateType.SUM, "CodesMovedTo");
            var codesMovedToCase = codesMovedToExpression.CaseExpression();
            var codesMovedToCondition = codesMovedToCase.AddCase();
            var movedToWhen = codesMovedToCondition.When();
            movedToWhen.EqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ChangeType).Value((int)CodeChangeType.Moved);
            movedToWhen.NotEqualsCondition(COL_ZoneChangeType).Value((int)ZoneChangeType.Deleted);
            movedToWhen.NotEqualsCondition(COL_ZoneChangeType).Value((int)ZoneChangeType.PendingClosed);
            movedToWhen.EqualsCondition(TABLE_ALIAS, COL_ZoneName, saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ZoneName);
            codesMovedToCondition.Then().Value(1);
            codesMovedToCase.Else().Value(0);

            var codesMovedFromExpression = aggregates.ExpressionAggregate(RDBNonCountAggregateType.SUM, "CodesMovedFrom");
            var codesMovedFromCase = codesMovedFromExpression.CaseExpression();
            var codesMovedFromCondition = codesMovedFromCase.AddCase();
            var movedFromWhen = codesMovedFromCondition.When();
            movedFromWhen.EqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ChangeType).Value((int)CodeChangeType.Moved);
            movedFromWhen.NotEqualsCondition(COL_ZoneChangeType).Value((int)ZoneChangeType.PendingClosed);
            movedFromWhen.EqualsCondition(TABLE_ALIAS, COL_ZoneName, saleCodePreviewAlias, SaleCodePreviewDataManager.COL_RecentZoneName);
            codesMovedFromCondition.Then().Value(1);
            codesMovedFromCase.Else().Value(0);

            saleCodePreviewDataManager.SetJoinContext(selectQuery.Join(), saleCodePreviewAlias,TABLE_ALIAS, COL_ZoneName);

            var where = selectQuery.Where();
            if (query.CountryId.HasValue)
                where.EqualsCondition(COL_CountryID).Value(query.CountryId.Value);
            if (!query.OnlyModified)
                where.NotEqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ChangeType).Value((int)CodeChangeType.NotChanged);
            where.EqualsCondition(COL_ProcessInstanceID).Value(query.ProcessInstanceId);
            where.EqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ProcessInstanceID).Value(query.ProcessInstanceId);

            var groupBy = selectQuery.GroupBy();
            groupBy.Select().Columns(COL_ZoneName, COL_RecentZoneName, COL_ZoneChangeType, COL_ZoneBED, COL_ZoneEED);

            return queryContext.GetItems(ZonePreviewMapper);
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', _columns);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(ZonePreview record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(_processInstanceID);
            recordContext.Value(record.CountryId);
            recordContext.Value(record.ZoneName);
            recordContext.Value(record.RecentZoneName);
            recordContext.Value((int)record.ChangeTypeZone);
            recordContext.Value(record.ZoneBED);
            if (record.ZoneEED.HasValue)
                recordContext.Value(record.ZoneEED.Value);
            else
                recordContext.Null();
        }
        #endregion

        #region CodePreparation
        public void SetJoinContext(RDBJoinContext joinContext, string joinedTableAlias, string originalTableAlias, string zoneNameColumn, string processInstanceIdColumn)
        {
            var joinCondition = joinContext.Join(TABLE_NAME, joinedTableAlias).On();
            joinCondition.EqualsCondition(joinedTableAlias, COL_ZoneName, originalTableAlias, zoneNameColumn);
            joinCondition.EqualsCondition(joinedTableAlias, COL_ProcessInstanceID, originalTableAlias, processInstanceIdColumn);
        }
        public void DeleteRecords(RDBDeleteQuery deleteQuery, long processInstanceId)
        {
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
        }

        public void BuildSelectQuery(RDBQueryContext queryContext, SPLPreviewQuery query)
        {
            string saleCodePreviewAlias = "saleCodePrev";
            var saleCodePreviewDataManager = new SaleCodePreviewDataManager();

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_CountryID, RDBDataType.Int);
            tempTableQuery.AddColumn("NewZones", RDBDataType.Int);
            tempTableQuery.AddColumn("DeletedZones", RDBDataType.Int);
            tempTableQuery.AddColumn("RenamedZones", RDBDataType.Int);
            tempTableQuery.AddColumn("NewCodes", RDBDataType.Int);
            tempTableQuery.AddColumn("MovedCodes", RDBDataType.Int);
            tempTableQuery.AddColumn("DeletedCodes", RDBDataType.Int);
            tempTableQuery.AddColumn("ZoneWithCodeChanges", RDBDataType.Varchar);

            var insertIntoTempTableQuery = queryContext.AddInsertQuery();
            insertIntoTempTableQuery.IntoTable(tempTableQuery);

            var selectQuery = insertIntoTempTableQuery.FromSelect();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var aggregates = selectQuery.SelectAggregates();
            selectQuery.SelectColumns().Column(COL_ZoneName, "ZoneWithCodeChanges");

            var newCodesExpression = aggregates.ExpressionAggregate(RDBNonCountAggregateType.SUM, "NewCodes");
            var newCodesCase = newCodesExpression.CaseExpression();
            var newCodesCondition = newCodesCase.AddCase();
            newCodesCondition.When().EqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ChangeType).Value((int)CodeChangeType.New);
            newCodesCondition.Then().Value(1);
            newCodesCase.Else().Value(0);

            var movedCodesExpression = aggregates.ExpressionAggregate(RDBNonCountAggregateType.SUM, "MovedCodes");
            var movedCodesCase = movedCodesExpression.CaseExpression();
            var movedCodesCondition = movedCodesCase.AddCase();
            var movedCodesWhen = movedCodesCondition.When();
            movedCodesWhen.EqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ChangeType).Value((int)CodeChangeType.Moved);
            movedCodesWhen.NotEqualsCondition(COL_ZoneChangeType).Value((int)ZoneChangeType.Deleted);
            movedCodesWhen.EqualsCondition(TABLE_ALIAS, COL_ZoneName, saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ZoneName);
            movedCodesCondition.Then().Value(1);
            movedCodesCase.Else().Value(0);

            var deletedCodesExpression = aggregates.ExpressionAggregate(RDBNonCountAggregateType.SUM, "DeletedCodes");
            var deletedCodesCase = deletedCodesExpression.CaseExpression();
            var deletedCodesCondition = deletedCodesCase.AddCase();
            deletedCodesCondition.When().EqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ChangeType).Value((int)CodeChangeType.Deleted);
            deletedCodesCondition.Then().Value(1);
            deletedCodesCase.Else().Value(0);

            saleCodePreviewDataManager.SetJoinContext(selectQuery.Join(), saleCodePreviewAlias, TABLE_ALIAS, COL_ZoneName);

            var where = selectQuery.Where();
            if (!query.OnlyModified)
                where.NotEqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ChangeType).Value((int)CodeChangeType.NotChanged);

            where.EqualsCondition(COL_ProcessInstanceID).Value(query.ProcessInstanceId);
            where.EqualsCondition(saleCodePreviewAlias, SaleCodePreviewDataManager.COL_ProcessInstanceID).Value(query.ProcessInstanceId);
            selectQuery.GroupBy().Select().Column(COL_ZoneName);

            var selectQuery2 = queryContext.AddSelectQuery();
            selectQuery2.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery2.SelectColumns().Column(COL_CountryID);
            var aggregates2 = selectQuery2.SelectAggregates();

            var newZonesExpression = aggregates2.ExpressionAggregate(RDBNonCountAggregateType.SUM, "NewZones");
            var newZonesCase = newZonesExpression.CaseExpression();
            var newZonesCondition = newZonesCase.AddCase();
            newZonesCondition.When().EqualsCondition(COL_ZoneChangeType).Value((int)ZoneChangeType.New);
            newZonesCondition.Then().Value(1);
            newZonesCase.Else().Value(0);

            var deletedZonesExpression = aggregates2.ExpressionAggregate(RDBNonCountAggregateType.SUM, "DeletedZones");
            var deletedZonesCase = deletedZonesExpression.CaseExpression();
            var deletedZonesCondition = deletedZonesCase.AddCase();
            deletedZonesCondition.When().EqualsCondition(COL_ZoneChangeType).Value((int)ZoneChangeType.Deleted);
            deletedZonesCondition.Then().Value(1);
            deletedZonesCase.Else().Value(0);

            var renamedZonesExpression = aggregates2.ExpressionAggregate(RDBNonCountAggregateType.SUM, "RenamedZones");
            var renamedZonesCase = renamedZonesExpression.CaseExpression();
            var renamedZonesCondition = renamedZonesCase.AddCase();
            renamedZonesCondition.When().EqualsCondition(COL_ZoneChangeType).Value((int)ZoneChangeType.Renamed);
            renamedZonesCondition.Then().Value(1);
            renamedZonesCase.Else().Value(0);

            var newCodesExpression2 = aggregates2.ExpressionAggregate(RDBNonCountAggregateType.SUM, "NewCodes");
            newCodesExpression2.Column("temp", "NewCodes");

            var movedCodesExpression2 = aggregates2.ExpressionAggregate(RDBNonCountAggregateType.SUM, "MovedCodes");
            movedCodesExpression2.Column("temp", "MovedCodes");


            var deletedCodesExpression2 = aggregates2.ExpressionAggregate(RDBNonCountAggregateType.SUM, "DeletedCodes");
            deletedCodesExpression2.Column("temp", "DeletedCodes");

            var tempTableJoinContext = selectQuery2.Join();
            var tempTableJoinCondition = tempTableJoinContext.Join(tempTableQuery, "temp").On();
            tempTableJoinCondition.EqualsCondition("temp", "ZoneWithCodeChanges", TABLE_ALIAS, COL_ZoneName);

            selectQuery2.Where().EqualsCondition(COL_ProcessInstanceID).Value(query.ProcessInstanceId);
            selectQuery2.GroupBy().Select().Column(COL_CountryID);
        }

        #endregion
    }
}
