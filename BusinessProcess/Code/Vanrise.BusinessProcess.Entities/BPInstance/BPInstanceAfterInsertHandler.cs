using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BPInstanceAfterInsertHandler
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IBPInstanceAfterInsertHandlerExecuteContext context);
    }

    public interface IBPInstanceAfterInsertHandlerExecuteContext
    {
        BPInstance BPInstance { get; }
        object CustomData { get; }
    }

    public class BPInstanceAfterInsertHandlerExecuteContext : IBPInstanceAfterInsertHandlerExecuteContext
    {
        public BPInstance BPInstance { get; set; }
        public object CustomData { get; set; }
    }

    public class TestingBPInstanceAfterInsertHandler : BPInstanceAfterInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("9DC2DA9E-9ACE-42B1-86FF-D7A7BEA20BDD"); } }

        public override void Execute(IBPInstanceBeforeInsertHandlerExecuteContext context)
        {
            throw new NotImplementedException();
        }
    }
}