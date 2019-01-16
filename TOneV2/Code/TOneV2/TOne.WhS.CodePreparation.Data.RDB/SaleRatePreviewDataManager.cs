using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class SaleRatePreviewDataManager : ISaleRatePreviewDataManager
    {

        #region RDB

        static string TABLE_NAME = "TOneWhs_CP_SaleRate_Preview";
        static string TABLE_ALIAS = "saleRatePrev";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_OwnerType = "OwnerType";
        const string COL_OwnerID = "OwnerID";
        const string COL_Rate = "Rate";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_ZoneName = "ZoneName";
        const string COL_CurrencyID = "CurrencyID";


        static SaleRatePreviewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_OwnerType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Rate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhs_CP",
                DBTableName = "SaleRate_Preview",
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
        private RatePreview RatePreviewMapper(IRDBDataReader reader)
        {
            RatePreview ratePreview = new RatePreview
            {
                OnwerType = (SalePriceListOwnerType)reader.GetInt(COL_OwnerType),
                OwnerId = reader.GetInt(COL_OwnerID),
                Rate = reader.GetDecimal(COL_Rate),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_BED),
                CurrencyId = reader.GetNullableInt(COL_CurrencyID)
            };
            return ratePreview;
        }
        #endregion
        #region ISaleRatePreviewDataManager
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;
        readonly string[] _columns = { COL_ProcessInstanceID, COL_ZoneName, COL_OwnerType, COL_OwnerID, COL_Rate, COL_BED, COL_EED, COL_CurrencyID };

        public void ApplyPreviewRatesToDB(object preparedRates)
        {
            preparedRates.CastWithValidate<RDBBulkInsertQueryContext>("preparedRates").Apply();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public IEnumerable<RatePreview> GetFilteredRatesPreview(SPLPreviewQuery query)//this method is not used anymore and hence was not tested
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var where = selectQuery.Where();
            if (query.ZoneName != null)
                where.EqualsCondition(COL_ZoneName).Value(query.ZoneName);
            where.EqualsCondition(COL_ProcessInstanceID).Value(query.ProcessInstanceId);
            return queryContext.GetItems(RatePreviewMapper);
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', _columns);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(RatePreview record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(_processInstanceID);
            recordContext.Value(record.ZoneName);
            recordContext.Value((int)record.OnwerType);
            recordContext.Value(record.OwnerId);
            recordContext.Value(decimal.Round(record.Rate, 8));
            recordContext.Value(record.BED);
            if (record.EED.HasValue)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Null();
            if (record.CurrencyId.HasValue)
                recordContext.Value(record.CurrencyId.Value);
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
        #endregion
    }
}
