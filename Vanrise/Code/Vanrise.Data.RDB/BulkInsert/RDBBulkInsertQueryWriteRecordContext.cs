using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBulkInsertQueryWriteRecordContext
    {
        BaseRDBStreamRecordForBulkInsert _currentRecord;

        public RDBBulkInsertQueryWriteRecordContext(BaseRDBStreamRecordForBulkInsert currentRecord)
        {
            _currentRecord = currentRecord;
        }

        public void Value(string value)
        {
            _currentRecord.Value(value);
        }

        public void Value(int value)
        {
            _currentRecord.Value(value);
        }

        public void Value(long value)
        {
            _currentRecord.Value(value);
        }

        public void Value(decimal value)
        {
            _currentRecord.Value(value);
        }

        public void Value(float value)
        {
            _currentRecord.Value(value);
        }

        public void Value(DateTime value)
        {
            _currentRecord.Value(value);
        }

        public void Value(bool value)
        {
            _currentRecord.Value(value);
        }

        public void Value(Guid value)
        {
            _currentRecord.Value(value);
        }
    }
}
