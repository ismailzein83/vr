using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBulkInsertQueryWriteRecordContext<T> : IRDBBulkInsertQueryWriteRecordContextValueDefined<T>
    {
        T _parent; 
        BaseRDBStreamRecordForBulkInsert _record;
        public RDBBulkInsertQueryWriteRecordContext(T parent, BaseRDBStreamForBulkInsert streamForBulkInsertContext)
        {
            _parent = parent;
            _record = streamForBulkInsertContext.CreateRecord();
        }

        public IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(string value)
        {
            _record.Value(value);
            return this;
        }

        public IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(int value)
        {
            _record.Value(value);
            return this;
        }

        public IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(long value)
        {
            _record.Value(value);
            return this;
        }

        public IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(decimal value)
        {
            _record.Value(value);
            return this;
        }

        public IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(float value)
        {
            _record.Value(value);
            return this;
        }

        public IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(DateTime value)
        {
            _record.Value(value);
            return this;
        }

        public IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(bool value)
        {
            _record.Value(value);
            return this;
        }

        public IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(Guid value)
        {
            _record.Value(value);
            return this;
        }

        public T EndWriteRecord()
        {
            _record.EndRecord();
            return _parent;
        }
    }

    public interface IRDBBulkInsertQueryWriteRecordContextValueDefined<T> : IRDBBulkInsertQueryWriteRecordContextCanDefineValue<T>, IRDBBulkInsertQueryWriteRecordContextCanEndWriteRecord<T>
    {

    }

    public interface IRDBBulkInsertQueryWriteRecordContextCanDefineValue<T>
    {
        IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(string value);

        IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(int value);

        IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(long value);

        IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(decimal value);

        IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(float value);

        IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(DateTime value);

        IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(bool value);

        IRDBBulkInsertQueryWriteRecordContextValueDefined<T> Value(Guid value);
    }

    public interface IRDBBulkInsertQueryWriteRecordContextCanEndWriteRecord<T>
    {
        T EndWriteRecord();
    }
}
