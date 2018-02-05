using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBStreamForBulkInsert
    {
        public abstract void WriteRecord(params Object[] values);

        public abstract void CloseStream();

        public abstract void Apply();
    }
}
