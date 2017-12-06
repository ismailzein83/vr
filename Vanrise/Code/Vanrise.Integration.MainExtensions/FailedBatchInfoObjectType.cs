using System;
using Vanrise.Entities;

namespace Vanrise.Integration.MainExtensions
{
    public class FailedBatchInfoObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("EA1F0775-08B9-4638-AC42-A2FD11E008EF"); }
        }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            throw new NotImplementedException();
        }
    }
}
