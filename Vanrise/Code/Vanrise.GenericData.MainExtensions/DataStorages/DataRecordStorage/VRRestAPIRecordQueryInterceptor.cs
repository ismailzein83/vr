using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataStorages.DataRecordStorage
{
    public class VRRestAPIRecordQueryInterceptor : DataRecordQueryInterceptor
    {
        //public override Guid ConfigId
        //{
        //    get { return new Guid("B3A94A20-92ED-47BF-86D6-1034B720BE73"); }
        //}

        //public override void PrepareQuery(IDataRecordQueryInterceptorContext context)
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class VRRestAPIRecordQueryInterceptorContext : IDataRecordQueryInterceptorContext
    {
        //public DataRecordQuery Query { get; set; }
    }
}
