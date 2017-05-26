using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.Business.CarrierAccounts
{
    public abstract class BaseCarrierAccountEventPayload : VREventPayload
    {
        public override string GetEventTypeUniqueName()
        {
            return BuildEventTypeUniqueName(GetEventType());
        }
        public abstract string GetEventType();
        public static string BuildEventTypeUniqueName(string eventType)
        {
            return string.Format("WhS_BE_CarrierAccount_{0}", eventType);
        }
    }

    public class CarrierAccountStatusChangedEventPayload : BaseCarrierAccountEventPayload
    {
        public int CarrierAccountId { get; set; }

        public const string S_EventType = "StatusChanged";
        public override string GetEventType()
        {
            return S_EventType;
        }
    }
    public abstract class BaseCarrierAccountEventHandler : VREventHandlerExtendedSettings
    {
        public override string GetEventTypeUniqueName()
        {
            return BaseCarrierAccountEventPayload.BuildEventTypeUniqueName(GetEventType());
        }
        protected abstract string GetEventType();
    }
    public abstract class CarrierAccountStatusChangedEventHandler : BaseCarrierAccountEventHandler
    {
        protected override string GetEventType()
        {
            return CarrierAccountStatusChangedEventPayload.S_EventType;
        }
    }
   
    //public class AccountBalanceCarrierAccountStatusChangedHandler : CarrierAccountStatusChangedEventHandler
    //{
    //    public Entities.ActivationStatus ActivationStatus { get; set; }
    //    public override Guid ConfigId
    //    {
    //        get { return new Guid("3F888397-58AA-49B8-9973-2272830BB8DD"); }
    //    }

    //    public override void Execute(IVREventHandlerContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
