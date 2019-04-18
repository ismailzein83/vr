using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.RDB
{
    class CustomerMappingSucceededDataManager : ICustomerMappingSucceededDataManager
    {
        const string CustomerMappingSucceededTableName = "CustomerMapping_Succeeded";
        string TABLE_Schema;
        string TABLE_NAME;

        const string COL_BO = "BO";
        const string COL_CustomerMapping = "CustomerMapping";
        const string COL_Action = "Action";

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        public string SwitchId { get; set; }

        public void SaveCustomerMappingsSucceededToDB(IEnumerable<CustomerMappingWithActionType> customerMappingsSucceeded)
        {
            if (customerMappingsSucceeded == null || !customerMappingsSucceeded.Any())
                return;

            Object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (var customerMappingSucceeded in customerMappingsSucceeded)
                WriteRecordToStream(customerMappingSucceeded, dbApplyStream);

            Object preparedInvalidCDRs = FinishDBApplyStream(dbApplyStream);

            ApplyCustomerMappingSucceededForDB(preparedInvalidCDRs);
        }

        public object InitialiazeStreamForDBApply()
        {
            TryRegisterTable(CustomerMappingSucceededTableName);

            var queryContext = new RDBQueryContext(GetDataProvider());

            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_BO, COL_CustomerMapping, COL_Action);

            return streamForBulkInsert;
        }

        public void WriteRecordToStream(CustomerMappingWithActionType record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            if (record.CustomerMapping.BO != null)
                recordContext.Value(record.CustomerMapping.BO);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(Helper.SerializeCustomerMapping(record.CustomerMapping));

            recordContext.Value((int)record.ActionType);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public void ApplyCustomerMappingSucceededForDB(object preparedCustomerMapping)
        {
            preparedCustomerMapping.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        private void TryRegisterTable(string TableName)
        {
            TABLE_Schema = ($"WhS_RouteSync_Ericsson_{SwitchId}");
            TABLE_NAME = ($"{TABLE_Schema}.[{TableName}]");

            RDBSchemaManager.Current.TryRegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = TABLE_Schema,
                DBTableName = TableName,
                Columns = GetRDBTableColumnDefinitionDictionary()
            });
        }

        private Dictionary<string, RDBTableColumnDefinition> GetRDBTableColumnDefinitionDictionary()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_BO, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_CustomerMapping, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_Action, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            return columns;
        }
    }
}
