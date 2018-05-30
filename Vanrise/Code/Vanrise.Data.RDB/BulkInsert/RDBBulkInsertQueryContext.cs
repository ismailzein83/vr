using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBulkInsertQueryContext : IRDBBulkInsertQueryContext, IRDBBulkInsertTableDefined, IRDBBulkInsertRecordWritten, IRDBBulkInsertBulkStreamClosed, IRDBBulkInsertBulkApplied
    {
        RDBQueryBuilderContext _queryBuilderContext;

        public RDBBulkInsertQueryContext(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        BaseRDBStreamForBulkInsert _streamForBulkInsertContext;

        public IRDBBulkInsertTableDefined IntoTable(string tableName, char fieldSeparator, string[] columnNames)
        {
            var initializeStreamForBulkInsertContext = new RDBDataProviderInitializeStreamForBulkInsertContext(tableName, fieldSeparator, columnNames);
            _streamForBulkInsertContext = _queryBuilderContext.DataProvider.InitializeStreamForBulkInsert(initializeStreamForBulkInsertContext);
            return this;
        }

        public RDBBulkInsertQueryWriteRecordContext<IRDBBulkInsertRecordWritten> WriteRecord()
        {
            return new RDBBulkInsertQueryWriteRecordContext<IRDBBulkInsertRecordWritten>(this, _streamForBulkInsertContext);
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

    public interface IRDBBulkInsertRecordWritten : IRDBBulkInsertCanInsertRecord, IRDBBulkInsertBulkCanCloseStream
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

        RDBBulkInsertQueryWriteRecordContext<IRDBBulkInsertRecordWritten> WriteRecord();

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
