﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public abstract class SwitchRouteSynchronizer
    {
        public virtual Guid ConfigId { get; set; }

        public abstract void Initialize(ISwitchRouteSynchronizerInitializeContext context);

        public abstract void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context);

        public abstract Object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context);

        public abstract void Finalize(ISwitchRouteSynchronizerFinalizeContext context);

        public abstract void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        public virtual bool TryBlockCustomer(ITryBlockCustomerContext context)
        {
            return false;
        }
        public virtual bool TryUnBlockCustomer(ITryUnBlockCustomerContext context)
        {
            return false;
        }
    }

    public enum RouteSyncDeliveryMethod { Batches = 0, AllRoutes = 1 }

    public abstract class SwitchRouteSyncInitializationData
    {
        public abstract RouteSyncDeliveryMethod SupportedDeliveryMethod { get; }
    }

    public interface ISwitchRouteSynchronizerInitializeContext
    {
        RouteRangeType? RouteRangeType { get; }

        SwitchRouteSyncInitializationData InitializationData { set; }
    }

    public interface ISwitchRouteSynchronizerConvertRoutesContext
    {
        RouteRangeType? RouteRangeType { get; }

        RouteRangeInfo RouteRangeInfo { get; }

        SwitchRouteSyncInitializationData InitializationData { get; }

        List<Route> Routes { get; }

        List<ConvertedRoute> ConvertedRoutes { set; }

        List<string> InvalidRoutes { set; }
    }

    public interface ISwitchRouteSynchronizerFinalizeContext
    {
        RouteRangeType? RouteRangeType { get; }

        SwitchRouteSyncInitializationData InitializationData { get; }

        void WriteTrackingMessage(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args);

        string SwitchName { get; }

        int IndexesCommandTimeoutInSeconds { get; }
    }

    public interface ISwitchRouteSynchronizerPrepareDataForApplyContext
    {
        RouteRangeType? RouteRangeType { get; }

        RouteRangeInfo RouteRangeInfo { get; }

        SwitchRouteSyncInitializationData InitializationData { get; }

        List<ConvertedRoute> ConvertedRoutes { get; }
    }

    public interface ISwitchRouteSynchronizerApplyRoutesContext
    {
        Object PreparedItemsForApply { get; }
    }
    public interface ITryBlockCustomerContext
    {
        string CustomerId { get; set; }

        SwitchCustomerBlockingInfo SwitchBlockingInfo { get; set; }
    }
    public interface ITryUnBlockCustomerContext
    {
        string CustomerId { get; set; }

        SwitchCustomerBlockingInfo SwitchBlockingInfo { get; set; }
    }
    public class TryBlockCustomerContext : ITryBlockCustomerContext
    {
        public string CustomerId { get; set; }

        public SwitchCustomerBlockingInfo SwitchBlockingInfo { get; set; }
    }
    public class TryUnBlockCustomerContext : ITryUnBlockCustomerContext
    {
        public string CustomerId { get; set; }

        public SwitchCustomerBlockingInfo SwitchBlockingInfo { get; set; }
    }
}
