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

        //public virtual string GetStartProcessOutputCode(IBeforeInsertHandlerGetStartProcessOutputCodeContext context)
        //{
        //    return "public BPInstanceBeforeInsertHandler BPInstanceBeforeInsertHandler { get; set; }";
        //}

        //public virtual void SetStartProcessOutput(IBeforeInsertHandlerSetStartProcessOutputContext context)
        //{
        //    SetObjectProperty("BPInstanceBeforeInsertHandler", "1", context.StartProcessOutput as object);
        //}

        //private void SetObjectProperty(string propertyName, string value, object obj)
        //{
        //    System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
        //    // make sure object has the property we are after
        //    if (propertyInfo != null)
        //        propertyInfo.SetValue(obj, value, null);
        //}
    }

    public interface IBPInstanceBeforeInsertHandlerExecuteContext
    {
        BPInstanceToAdd BPInstanceToAdd { get; }
        object CustomData { set; }
    }

    public class BPInstanceBeforeInsertHandlerExecuteContext : IBPInstanceBeforeInsertHandlerExecuteContext
    {
        public BPInstanceToAdd BPInstanceToAdd { get; set; }
        public object CustomData { get; set; }
    }

    //public interface IBeforeInsertHandlerGetStartProcessOutputCodeContext
    //{
    //}

    //public class BeforeInsertHandlerGetStartProcessOutputCodeContext : IBeforeInsertHandlerGetStartProcessOutputCodeContext
    //{
    //}

    //public interface IBeforeInsertHandlerSetStartProcessOutputContext
    //{
    //    dynamic StartProcessOutput { get; set; }
    //}

    //public class BeforeInsertHandlerSetStartProcessOutputContext : IBeforeInsertHandlerSetStartProcessOutputContext
    //{
    //    public dynamic StartProcessOutput { get; set; }
    //}

    public class TestingBPInstanceBeforeInsertHandler : BPInstanceBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("371F8454-DAF0-4001-86FE-057B6BCDDA31"); } }

        public override void Execute(IBPInstanceBeforeInsertHandlerExecuteContext context)
        {
            throw new NotImplementedException();
        }
    }
}