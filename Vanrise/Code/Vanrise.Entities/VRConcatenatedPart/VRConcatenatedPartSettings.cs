using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRConcatenatedPartSettings<T> where T : class
    {
        public abstract Guid ConfigId { get; }
        public virtual void IntializePart(IConcatenatedPartInitializeContext context)
        {
        }
        public abstract string GetPartText(T context);
    }

    public interface IConcatenatedPartInitializeContext
    {
        Guid SequenceDefinitionId { get; set; }
        Object CustomData { get; set; }
        long NumberOfItems { get; set; }
    }
    public class ConcatenatedPartInitializeContext : IConcatenatedPartInitializeContext
    {
        public Guid SequenceDefinitionId { get; set; }
        public Object CustomData { get; set; }
        public long NumberOfItems { get; set; }
    }


}
