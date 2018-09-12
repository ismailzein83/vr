using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BPInstanceInsertHandler
    {
        public abstract Guid ConfigId { get; }

        public abstract void ExecuteBeforeInsert(IBPInstanceHandlerBeforeExecuteInsertContext context);

        public abstract void ExecuteAfterInsert(IBPInstanceHandlerAfterExecuteInsertContext context);

        public virtual void BeforeAPICompile(IBPInstanceHandlerBeforeAPICompileContext context)
        {
        }
    }

    public interface IBPInstanceHandlerBeforeExecuteInsertContext
    {
        BPInstanceToAdd BPInstanceToAdd { get; }
        dynamic StartProcessOutput { get; }

        /// <summary>
        /// If Needed, it can be used to pass data from BeforeInsertHandler to AfterInsertHandler 
        /// </summary>
        object CustomData { set; }
    }
    public class BPInstanceHandlerBeforeExecuteInsertContext : IBPInstanceHandlerBeforeExecuteInsertContext
    {
        public BPInstanceToAdd BPInstanceToAdd { get; set; }
        public dynamic StartProcessOutput { get; set; }
        public object CustomData { get; set; }
    }

    public interface IBPInstanceHandlerAfterExecuteInsertContext
    {
        BPInstance BPInstance { get; }
        dynamic StartProcessOutput { get; }
        object CustomData { get; }
    }
    public class BPInstanceHandlerAfterExecuteInsertContext : IBPInstanceHandlerAfterExecuteInsertContext
    {
        public BPInstance BPInstance { get; set; }
        public dynamic StartProcessOutput { get; set; }
        public object CustomData { get; set; }
    }

    public interface IBPInstanceHandlerBeforeAPICompileContext
    {
        string OutputArgumentCode { set; }
    }
    public class BPInstanceHandlerBeforeAPICompileContext : IBPInstanceHandlerBeforeAPICompileContext
    {
        public string OutputArgumentCode { get; set; }
    }
}