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
    }

    public class BPInstanceAfterInsertHandlerExecuteContext : IBPInstanceAfterInsertHandlerExecuteContext
    {
        public BPInstance BPInstance { get; set; }
    }
}
