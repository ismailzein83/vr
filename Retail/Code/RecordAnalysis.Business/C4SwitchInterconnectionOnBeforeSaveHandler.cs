using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace RecordAnalysis.Business
{
    public class C4SwitchInterconnectionOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("40FF1961-EC19-44AB-A738-0F32E54F5164"); } }

        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
