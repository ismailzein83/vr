using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBulkInsertQueryContext : IRDBBulkInsertQueryContext, IRDBBulkInsertTableDefined, IRDBBulkInsertBulkStreamClosed, IRDBBulkInsertBulkApplied
    {
        BaseRDBQueryContext _queryContext;

        public RDBBulkInsertQueryContext(BaseRDBQueryContext queryContext)
        {
            _queryContext = queryContext;
        }

        BaseRDBStreamForBulkInsert _streamForBulkInsertContext;

        public IRDBBulkInsertTableDefined IntoTable(string tableName, char fieldSeparator, string[] columnNames)
        {
            var initializeStreamForBulkInsertContext = new RDBDataProviderInitializeStreamForBulkInsertContext(tableName, fieldSeparator, columnNames);
            _streamForBulkInsertContext = _queryContext.DataProvider.InitializeStreamForBulkInsert(initializeStreamForBulkInsertContext);
            return this;
        }

        public IRDBBulkInsertTableDefined WriteRecord(params Object[] values)
        {
            _streamForBulkInsertContext.WriteRecord(values);
            return this;
        }

        public IRDBBulkInsertBulkStreamClosed CloseStream()
        {
            _streamForBulkInsertContext.CloseStream();
            return this;
        }

        public IRDBBulkInsertBulkApplied Apply()
        {
            _streamForBulkInsertContext.Apply();
            return this;
        }
    }

    public interface IRDBBulkInsertQueryContext : IRDBBulkInsertCanDefineTable
    {

    }

    public interface IRDBBulkInsertTableDefined : IRDBBulkInsertCanInsertRecord, IRDBBulkInsertBulkCanCloseStream
    {

    }

    public interface IRDBBulkInsertBulkStreamClosed : IRDBBulkInsertBulkCanApply
    {

    }

    public interface IRDBBulkInsertBulkApplied
    {

    }

    public interface IRDBBulkInsertCanDefineTable
    {
        IRDBBulkInsertTableDefined IntoTable(string tableName, char fieldSeparator, string[] columnNames);
    }

    public interface IRDBBulkInsertCanInsertRecord
    {
        IRDBBulkInsertTableDefined WriteRecord(params Object[] values);
    }
    
    public interface IRDBBulkInsertBulkCanCloseStream
    {
        IRDBBulkInsertBulkStreamClosed CloseStream();
    }

    public interface IRDBBulkInsertBulkCanApply
    {
        IRDBBulkInsertBulkApplied Apply();
    }
}
