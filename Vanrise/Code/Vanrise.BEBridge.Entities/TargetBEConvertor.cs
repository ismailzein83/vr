using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BEBridge.Entities
{
    public abstract class TargetBEConvertor
    {
        public virtual string Name { get { return "Base Target BE Convertor"; } }
        public Guid ConfigId { get; set; }

        public abstract void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context);

        public abstract void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context);

        public virtual bool CompareBeforeUpdate { get { return true; } }
    }

    public interface ITargetBEConvertorConvertSourceBEsContext
    {
        SourceBEBatch SourceBEBatch { get; }

        List<ITargetBE> TargetBEs { set; }

        void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args);

        void WriteBusinessTrackingMsg(LogEntryType severity, string messageFormat, params object[] args);

        void WriteHandledException(Exception ex);

        void WriteBusinessHandledException(Exception ex);
    }

    public interface ITargetBEConvertorMergeTargetBEsContext
    {
        ITargetBE ExistingBE { get; }

        ITargetBE NewBE { get; }

        ITargetBE FinalBE { set; }

        void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args);

        void WriteBusinessTrackingMsg(LogEntryType severity, string messageFormat, params object[] args);

        void WriteHandledException(Exception ex);

        void WriteBusinessHandledException(Exception ex);
    }
}
