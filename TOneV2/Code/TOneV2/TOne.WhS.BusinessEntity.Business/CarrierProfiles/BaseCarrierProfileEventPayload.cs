using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business.CarrierProfiles
{
    public abstract class BaseCarrierProfileEventPayload : VREventPayload
    {
        public override string GetEventTypeUniqueName()
        {
            return BuildEventTypeUniqueName(GetEventType());
        }
        public abstract string GetEventType();
        public static string BuildEventTypeUniqueName(string eventType)
        {
            return string.Format("WhS_BE_CarrierProfile_{0}", eventType);
        }
    }
    public class CarrierProfileStatusChangedEventPayload : BaseCarrierProfileEventPayload
    {
        public int CarrierProfileId { get; set; }

        public const string S_EventType = "StatusChanged";
        public override string GetEventType()
        {
            return S_EventType;
        }
    }
    public abstract class BaseCarrierProfileEventHandler : VREventHandlerExtendedSettings
    {
        public override string GetEventTypeUniqueName()
        {
            return BaseCarrierProfileEventPayload.BuildEventTypeUniqueName(GetEventType());
        }
        protected abstract string GetEventType();
    }
    public abstract class CarrierProfileStatusChangedEventHandler : BaseCarrierProfileEventHandler
    {
        protected override string GetEventType()
        {
            return CarrierProfileStatusChangedEventPayload.S_EventType;
        }
    }
}
