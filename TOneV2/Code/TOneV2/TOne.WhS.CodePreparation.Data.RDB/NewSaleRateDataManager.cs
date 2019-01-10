using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.RDB;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class NewSaleRateDataManager : INewSaleRateDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_BE_CP_SaleRate_New";
        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_ZoneID = "ZoneID";
        const string COL_PriceListID = "PriceListID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_NormalRate = "NormalRate";
        const string COL_BED = "BED";
        const string COL_EED = "EED";


        static NewSaleRateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_PriceListID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_NormalRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CP_SaleRate_New",
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

        #region INewSaleRateDataManager
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }
        long _processInstanceID;

        readonly string[] _columns = { COL_ID, COL_ProcessInstanceID, COL_ZoneID, COL_PriceListID, COL_NormalRate, COL_BED, COL_EED };

        public void ApplyNewRatesToDB(object preparedRates)
        {
            preparedRates.CastWithValidate<RDBBulkInsertQueryContext>("preparedRates").Apply();
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', _columns);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(AddedRate record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord(); 
            recordContext.Value(record.RateId);
            recordContext.Value(_processInstanceID);
            recordContext.Value(record.AddedZone.ZoneId);
            recordContext.Value(record.PriceListToAdd.PriceListId);
            recordContext.Value(decimal.Round(record.NormalRate, 8));
            recordContext.Value(record.BED);
            if (record.EED.HasValue)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Null();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        #endregion
    }
}
