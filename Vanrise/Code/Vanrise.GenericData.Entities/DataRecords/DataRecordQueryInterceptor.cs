using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordQueryInterceptor
    {
        public abstract Guid ConfigId { get; }
        public abstract void PrepareQuery(IDataRecordQueryInterceptorContext context);
    }

    public interface IDataRecordQueryInterceptorContext
    {
        DataRecordQuery Query { get; set; }
    }
}
