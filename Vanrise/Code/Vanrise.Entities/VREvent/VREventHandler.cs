using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VREventHandler : IDateEffectiveSettings
    {
        public Guid VREventHandlerId { get; set; }

        public string Name { get; set; }

        public VREventHandlerSettings Settings { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    


    //public abstract class BaseCarrierAccountEventPayload : VREventPayload
    //{
    //    public override string GetEventTypeUniqueName()
    //    {
    //        return BuildEventTypeUniqueName(GetEventType());
    //    }

    //    public abstract string GetEventType();

    //    public static string BuildEventTypeUniqueName(string eventType)
    //    {
    //        return string.Format("WhS_BE_CarrierAccount_{0}", eventType);
    //    }
    //}

    //public class CarrierAccountStatusChangedEventPayload : BaseCarrierAccountEventPayload
    //{
    //    public const string S_EventType = "StatusChanged";
        
    //    public override string GetEventType()
    //    {
    //        return S_EventType;
    //    }
    //}

    //public abstract class BaseCarrierAccountEventHandler : VREventHandlerExtendedSettings
    //{
    //    public override string GetEventTypeUniqueName()
    //    {
    //        return BaseCarrierAccountEventPayload.BuildEventTypeUniqueName(GetEventType());
    //    }

    //    protected abstract string GetEventType();
    //}

    //public abstract class CarrierAccountStatusChangedEventHandler : BaseCarrierAccountEventHandler
    //{
    //    protected override string GetEventType()
    //    {
    //        return CarrierAccountStatusChangedEventPayload.S_EventType;
    //    }
    //}

    //public abstract class BaseGenericRuleEventPayload : VREventPayload
    //{
    //    public override string GetEventTypeUniqueName()
    //    {
    //        return BuildEventTypeUniqueName(this.GetRuleDefinitionId(), this.GetEventType());
    //    }

    //    protected abstract string GetEventType();

    //    protected abstract Guid GetRuleDefinitionId();

    //    public static string BuildEventTypeUniqueName(Guid ruleDefinitionId, string eventType)
    //    {
    //        return string.Format("VR_GenericRule_{0}_{1}", ruleDefinitionId, eventType);
    //    }
    //}

    //public abstract class BaseGenericRuleEventHandler : VREventHandlerExtendedSettings
    //{
    //    public override string GetEventTypeUniqueName()
    //    {
    //        return BaseGenericRuleEventPayload.BuildEventTypeUniqueName(this.GetRuleDefinitionId(), this.GetEventType());
    //    }

    //    protected abstract string GetEventType();

    //    protected abstract Guid GetRuleDefinitionId();
    //}
}