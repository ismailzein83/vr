﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

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
        object TargetBEId { get; }
        ITargetBE TargetBE { set; }

        Object InitializationData { get; }

        void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args);

        void WriteBusinessTrackingMsg(LogEntryType severity, string messageFormat, params object[] args);

        void WriteHandledException(Exception ex);

        void WriteBusinessHandledException(Exception ex);
    }

    public interface ITargetBESynchronizerInsertBEsContext : ITargetBESynchronizerBEsContext
    {
        List<ITargetBE> TargetBE { get; }
        Object InitializationData { get; }


    }

    public interface ITargetBESynchronizerUpdateBEsContext : ITargetBESynchronizerBEsContext
    {
        List<ITargetBE> TargetBE { get; }

    }

    public interface ITargetBESynchronizerBEsContext
    {
        void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args);

        void WriteBusinessTrackingMsg(LogEntryType severity, string messageFormat, params object[] args);

        void WriteHandledException(Exception ex);

        void WriteBusinessHandledException(Exception ex);
    }
}
