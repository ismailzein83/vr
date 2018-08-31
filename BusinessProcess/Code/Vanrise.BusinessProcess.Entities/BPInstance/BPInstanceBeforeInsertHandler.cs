using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BPInstanceBeforeInsertHandler
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IBPInstanceBeforeInsertHandlerExecuteContext context);
    }

    public interface IBPInstanceBeforeInsertHandlerExecuteContext
    {
        BPInstanceToAdd BPInstanceToAdd { get; }
    }

    public class BPInstanceBeforeInsertHandlerExecuteContext : IBPInstanceBeforeInsertHandlerExecuteContext
    {
        public BPInstanceToAdd BPInstanceToAdd { get; set; } 
    }
}
