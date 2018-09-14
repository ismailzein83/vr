using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBStreamForBulkInsert
    {
        public abstract BaseRDBStreamRecordForBulkInsert CreateRecord();

        public abstract void CloseStream();

        public abstract void Apply();
    }

    public abstract class BaseRDBStreamRecordForBulkInsert
    {
        public abstract void Value(string value);

        public abstract void Value(int value);

        public abstract void Value(long value);

        public abstract void Value(decimal value);

        public abstract void Value(float value);

        public abstract void Value(DateTime? value);

        public abstract void Value(DateTime value);

        public abstract void ValueDateOnly(DateTime? value);

        public abstract void ValueDateOnly(DateTime value);

        public abstract void Value(Vanrise.Entities.Time value);

        public abstract void Value(bool value);

        public abstract void Value(Guid value);

        public abstract void WriteRecord();
    }
}