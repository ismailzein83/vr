using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public abstract class SwitchRouteSynchronizer
    {
        public abstract Guid ConfigId { get; }

        public virtual bool SupportSyncWithinRouteBuild { get { return true; } }

        public abstract void Initialize(ISwitchRouteSynchronizerInitializeContext context);

        public abstract void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context);

        public virtual void onAllRoutesConverted(ISwitchRouteSynchronizerOnAllRoutesConvertedContext context)
        {

        }

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

        public abstract void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context);
    }

    public enum RouteSyncDeliveryMethod { Batches = 0, AllRoutes = 1 }

    public abstract class SwitchRouteSyncInitializationData
    {
        public abstract RouteSyncDeliveryMethod SupportedDeliveryMethod { get; }
    }

    public interface ISwitchRouteSynchronizerInitializeContext
    {
        string SwitchId { get; }

        string SwitchName { get; }

        RouteRangeType? RouteRangeType { get; }

        Action<Exception, bool> WriteBusinessHandledException { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }

        SwitchRouteSyncInitializationData InitializationData { set; }
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

        string SwitchId { get; }
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

	public class SwitchRouteSynchronizerApplyRoutesContext : ISwitchRouteSynchronizerApplyRoutesContext
	{
		public SwitchRouteSynchronizerApplyRoutesContext(Object preparedItemsForApply, string switchName, string switchId, SwitchSyncOutput previousSwitchSyncOutput, Action<Exception, bool> writeBusinessHandledException)
		{
			PreparedItemsForApply = preparedItemsForApply;
			SwitchId = switchId;
			WriteBusinessHandledException = writeBusinessHandledException;
			PreviousSwitchSyncOutput = previousSwitchSyncOutput;
			SwitchName = switchName;
		}
		public Object PreparedItemsForApply { get; set; }

		public string SwitchId { get; set; }

		public SwitchSyncOutput SwitchSyncOutput { get; set; }

		public SwitchSyncOutput PreviousSwitchSyncOutput { get; set; }

		public Action<Exception, bool> WriteBusinessHandledException { get; set; }

		public string SwitchName { get; set; }
	}

	public interface ISwitchRouteSynchronizerConvertRoutesContext
    {
        string SwitchId { get; }

        RouteRangeType? RouteRangeType { get; }

        RouteRangeInfo RouteRangeInfo { get; }

        SwitchRouteSyncInitializationData InitializationData { get; }

        List<Route> Routes { get; }

        List<ConvertedRoute> ConvertedRoutes { set; }

        Object ConvertedRoutesPayload { get; set; }
    }
    public class SwitchRouteSynchronizerConvertRoutesContext : ISwitchRouteSynchronizerConvertRoutesContext
    {
        public string SwitchId { get; set; }

        public RouteRangeType? RouteRangeType { get; set; }

        public RouteRangeInfo RouteRangeInfo { get; set; }

        public SwitchRouteSyncInitializationData InitializationData { get; set; }

        public List<Route> Routes { get; set; }

        public List<ConvertedRoute> ConvertedRoutes { get; set; }

        public object ConvertedRoutesPayload { get; set; }
    }

    public interface ISwitchRouteSynchronizerOnAllRoutesConvertedContext
    {
        Object ConvertedRoutesPayload { get; }

        List<ConvertedRoute> ConvertedRoutes { set; }
    }
    public class SwitchRouteSynchronizerOnAllRoutesConvertedContext : ISwitchRouteSynchronizerOnAllRoutesConvertedContext
    {
        public Object ConvertedRoutesPayload { get; set; }

        public List<ConvertedRoute> ConvertedRoutes { get; set; }
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
        Func<int, string> GetCarrierAccountNameById { get; }
        List<string> ValidationMessages { set; }
    }
    public class IsSwitchRouteSynchronizerValidContext : IIsSwitchRouteSynchronizerValidContext
    {
        public List<string> ValidationMessages { get; set; }

        public Func<int, string> GetCarrierAccountNameById { get; set; }
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

    public interface ISwitchRouteSynchronizerRemoveConnectionContext { }

    public class SwitchRouteSynchronizerRemoveConnectionContext : ISwitchRouteSynchronizerRemoveConnectionContext { }
}
