using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Data.RDB;
using Vanrise.Common;
using Vanrise.Entities;
namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class NewSaleCodeDataManager : INewSaleCodeDataManager
    {
        #region RDB

        static string TABLE_NAME = "TOneWhS_BE_CP_SaleCode_New";
        static string TABLE_ALIAS = "scnew";
        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_Code = "Code";
        const string COL_ZoneID = "ZoneID";
        const string COL_CodeGroupID = "CodeGroupID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";


        static NewSaleCodeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CodeGroupID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CP_SaleCode_New",
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

        #region INewSaleCodeDataManager
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;
        readonly string[] _columns = { COL_ID, COL_ProcessInstanceID, COL_Code, COL_ZoneID, COL_CodeGroupID, COL_BED, COL_EED };

        public void ApplyNewCodesToDB(object preparedCodes)
        {
            preparedCodes.CastWithValidate<RDBBulkInsertQueryContext>("preparedCodes").Apply();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', _columns);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(AddedCode record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(record.CodeId);
            recordContext.Value(_processInstanceID);
            recordContext.Value(record.Code);
            recordContext.Value(record.Zone.ZoneId);
            recordContext.Value(record.CodeGroupId);
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

        public void BuildSelectQuery(RDBSelectQuery selectQuery, long processInstanceId)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_Code, COL_ZoneID, COL_CodeGroupID, COL_BED, COL_EED);
            selectQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
        }
        #endregion
    }
}
