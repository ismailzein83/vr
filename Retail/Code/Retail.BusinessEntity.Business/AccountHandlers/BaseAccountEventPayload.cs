using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public abstract class BaseAccountEventPayload : VREventPayload
    {
        public override string GetEventTypeUniqueName()
        {
            return BuildEventTypeUniqueName(GetEventType());
        }
        public abstract string GetEventType();
        public static string BuildEventTypeUniqueName(string eventType)
        {
            return string.Format("Retail_BE_Account_{0}", eventType);
        }
    }
    public class AccountStatusChangedEventPayload : BaseAccountEventPayload
    {
        public long AccountId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }

        public const string S_EventType = "StatusChanged";
        public override string GetEventType()
        {
            return string.Format("{0}_{1}",S_EventType,this.AccountBEDefinitionId);
        }
    }
    public abstract class BaseAccountEventHandler : VREventHandlerExtendedSettings
    {
        public override string GetEventTypeUniqueName()
        {
            return BaseAccountEventPayload.BuildEventTypeUniqueName(GetEventType());
        }
        protected abstract string GetEventType();
    }
    public abstract class AccountStatusChangedEventHandler : BaseAccountEventHandler
    {
        protected override string GetEventType()
        {
            return AccountStatusChangedEventPayload.S_EventType;
        }
    }
}
