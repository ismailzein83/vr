using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Entities
{
    public abstract class VRRestAPIRecordQueryInterceptor 
    {
        public abstract Guid ConfigId { get; }
        public abstract void PrepareQuery(IVRRestAPIRecordQueryInterceptorContext context);
    }

    public interface IVRRestAPIRecordQueryInterceptorContext
    { 
        DataRecordQuery Query { get; set; }
    }
}
