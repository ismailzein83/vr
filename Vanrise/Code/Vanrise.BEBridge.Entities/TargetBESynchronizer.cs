using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public abstract class TargetBESynchronizer
    {
        public Guid ConfigId { get; set; }

        public abstract bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context);

        public abstract void InsertBEs(ITargetBESynchronizerInsertBEsContext context);

        public abstract void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context);
    }

    public interface ITargetBESynchronizerTryGetExistingBEContext
    {
        object SourceBEId { get; }

        ITargetBE TargetBE { set; }
    }

    public interface ITargetBESynchronizerInsertBEsContext
    {
        List<ITargetBE> TargetBE { get; }
    }

    public interface ITargetBESynchronizerUpdateBEsContext
    {
        List<ITargetBE> TargetBE { get; }
    }
}
