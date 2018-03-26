using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public abstract class SwitchRouteSynchronizer
    {
        public abstract Guid ConfigId { get; }

        public abstract void Initialize(ISwitchRouteSynchronizerInitializeContext context);

        public abstract void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context);

        public abstract Object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context);

        public abstract void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);

        public abstract void Finalize(ISwitchRouteSynchronizerFinalizeContext context);

        public virtual bool TryBlockCustomer(ITryBlockCustomerContext context)
        {
            return false;
        }

        public virtual bool TryUnBlockCustomer(ITryUnBlockCustomerContext context)
        {
            return false;
        }

        public virtual bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            return true;
        }

        public virtual bool SupportPartialRouteSync { get { return false; } }

        public virtual void ApplyDifferentialRoutes(ISwitchRouteSynchronizerApplyDifferentialRoutesContext context)
        {
            throw new NotImplementedException();
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

        string SwitchName { get; }

        string SwitchId { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }

        Action<Exception, bool> WriteBusinessHandledException { get; }
    }

    public interface ISwitchRouteSynchronizerFinalizeContext
    {
        RouteRangeType? RouteRangeType { get; }

        SwitchRouteSyncInitializationData InitializationData { get; }

        Action<LogEntryType, string, object[]> WriteTrackingMessage { get; }

        string SwitchName { get; }

        int IndexesCommandTimeoutInSeconds { get; }

        string SwitchId { get; }

        SwitchSyncOutput PreviousSwitchSyncOutput { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }

        Action<Exception, bool> WriteBusinessHandledException { get; }
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

        //Action<LogEntryType, string, object[]> WriteTrackingMessage { get; }

        string SwitchName { get; }

        string SwitchId { get; }

        SwitchSyncOutput PreviousSwitchSyncOutput { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }

        Action<Exception, bool> WriteBusinessHandledException { get; }
    }

    public interface ISwitchRouteSynchronizerConvertRoutesContext
    {
        RouteRangeType? RouteRangeType { get; }

        RouteRangeInfo RouteRangeInfo { get; }

        SwitchRouteSyncInitializationData InitializationData { get; }

        List<Route> Routes { get; }

        List<ConvertedRoute> ConvertedRoutes { set; }
    }
    public class SwitchRouteSynchronizerConvertRoutesContext : ISwitchRouteSynchronizerConvertRoutesContext
    {
        public RouteRangeType? RouteRangeType { get; set; }

        public RouteRangeInfo RouteRangeInfo { get; set; }

        public SwitchRouteSyncInitializationData InitializationData { get; set; }

        public List<Route> Routes { get; set; }

        public List<ConvertedRoute> ConvertedRoutes { get; set; }
    }

    public interface ISwitchRouteSynchronizerApplyDifferentialRoutesContext
    {
        string SwitchId { get; }

        string SwitchName { get; }

        List<Route> UpdatedRoutes { get; }

        Action<Exception, bool> WriteBusinessHandledException { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }

    }
    public class SwitchRouteSynchronizerApplyDifferentialRoutesContext : ISwitchRouteSynchronizerApplyDifferentialRoutesContext
    {
        public string SwitchId { get; set; }

        public string SwitchName { get; set; }

        public List<Route> UpdatedRoutes { get; set; }

        public Action<Exception, bool> WriteBusinessHandledException { get; set; }

        public SwitchSyncOutput SwitchSyncOutput { get; set; }
    }

    public interface ITryBlockCustomerContext
    {
        string SwitchName { get; }

        string CustomerId { get; }

        object SwitchBlockingInfo { set; }
    }
    public class TryBlockCustomerContext : ITryBlockCustomerContext
    {
        public string SwitchName { get; set; }

        public string CustomerId { get; set; }

        public object SwitchBlockingInfo { get; set; }
    }

    public interface ITryUnBlockCustomerContext
    {
        string CustomerId { get; }

        object SwitchBlockingInfo { set; }
    }
    public class TryUnBlockCustomerContext : ITryUnBlockCustomerContext
    {
        public string CustomerId { get; set; }

        public object SwitchBlockingInfo { get; set; }
    }

    public interface IIsSwitchRouteSynchronizerValidContext
    {
        List<string> ValidationMessages { set; }
    }
    public class IsSwitchRouteSynchronizerValidContext : IIsSwitchRouteSynchronizerValidContext
    {
        public List<string> ValidationMessages { get; set; }
    }
}
