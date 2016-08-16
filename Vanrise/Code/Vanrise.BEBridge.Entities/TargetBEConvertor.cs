using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public abstract class TargetBEConvertor
    {
        public int ConfigId { get; set; }

        public abstract void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context);

        public abstract void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context);

        public virtual bool CompareBeforeUpdate { get { return true; } }
    }

    public interface ITargetBEConvertorConvertSourceBEsContext
    {
        SourceBEBatch SourceBEBatch { get; }

        List<ITargetBE> TargetBEs { set; }
    }

    public interface ITargetBEConvertorMergeTargetBEsContext
    {
        ITargetBE ExistingBE { get; }

        ITargetBE NewBE { get; }

        ITargetBE FinalBE { set; }
    }
}
