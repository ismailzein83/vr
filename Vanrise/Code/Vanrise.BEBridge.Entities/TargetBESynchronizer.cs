using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public abstract class TargetBESynchronizer
    {
        public virtual string Name { get { return "Target BE Synchronizer"; } }
        public Guid ConfigId { get; set; }

        public virtual void Initialize(ITargetBESynchronizerInitializeContext context)
        {

        }

        public abstract bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context);

        public abstract void InsertBEs(ITargetBESynchronizerInsertBEsContext context);

        public abstract void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context);
    }

    public interface ITargetBESynchronizerInitializeContext
    {
        Object InitializationData { set; }
    }

    public interface ITargetBESynchronizerTryGetExistingBEContext
    {
        object SourceBEId { get; }

        ITargetBE TargetBE { set; }

        Object InitializationData { get; }
    }

    public interface ITargetBESynchronizerInsertBEsContext
    {
        List<ITargetBE> TargetBE { get; }
        Object InitializationData { get; }
    }

    public interface ITargetBESynchronizerUpdateBEsContext
    {
        List<ITargetBE> TargetBE { get; }
    }
}
