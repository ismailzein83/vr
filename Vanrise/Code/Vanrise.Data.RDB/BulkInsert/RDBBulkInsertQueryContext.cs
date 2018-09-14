using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBulkInsertQueryContext
    {
        RDBQueryBuilderContext _queryBuilderContext;

        public RDBBulkInsertQueryContext(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        BaseRDBStreamForBulkInsert _streamForBulkInsertContext;

        public void IntoTable(string tableName, char fieldSeparator, params string[] columnNames)
        {
            IntoTable(RDBSchemaManager.Current, tableName, fieldSeparator, columnNames);
        }

        public void IntoTable(RDBSchemaManager schemaManager, string tableName, char fieldSeparator, params string[] columnNames)
        {
            var initializeStreamForBulkInsertContext = new RDBDataProviderInitializeStreamForBulkInsertContext(_queryBuilderContext.DataProvider, schemaManager, tableName, fieldSeparator, columnNames);
            _streamForBulkInsertContext = _queryBuilderContext.DataProvider.InitializeStreamForBulkInsert(initializeStreamForBulkInsertContext);
        }

        BaseRDBStreamRecordForBulkInsert _currentRecord;

        public RDBBulkInsertQueryWriteRecordContext WriteRecord()
        {
            if (_currentRecord != null)
                _currentRecord.WriteRecord();

            _currentRecord = _streamForBulkInsertContext.CreateRecord();
            return new RDBBulkInsertQueryWriteRecordContext(_currentRecord);
        }

        public void CloseStream()
        {
            if (_currentRecord != null)
                _currentRecord.WriteRecord();
            _streamForBulkInsertContext.CloseStream();
        }

        public void Apply()
        {
            _streamForBulkInsertContext.Apply();
        }
    }

    
}
