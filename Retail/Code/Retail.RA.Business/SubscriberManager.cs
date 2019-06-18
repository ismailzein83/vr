using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Retail.RA.Entities;

namespace Retail.RA.Business
{
    public class SubscriberManager
    {
        static Guid s_subscriberBEDefinitionId = new Guid("5f8af839-22bb-4c1a-92a4-79817ec15126");
        public static Dictionary<string, SubscriberItem> GetSubscribersByMSISDN(List<MSISDNItem> msisdnDefinitions)
        {
            var subscribers = new Dictionary<string, SubscriberItem>();
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            var filterGroup = new RecordFilterGroup()
            {
                LogicalOperator = RecordQueryLogicalOperator.Or,
                Filters = new List<RecordFilter>()
            };
            if (msisdnDefinitions != null && msisdnDefinitions.Count > 0)
            {
                foreach (var item in msisdnDefinitions)
                {
                    filterGroup.Filters.Add(
                    new StringRecordFilter() { FieldName = "MSISDN", CompareOperator = StringRecordFilterOperator.Contains, Value = item.MSISDN });
                }
            }
            var allSubscribers = genericBusinessEntityManager.GetAllGenericBusinessEntities(s_subscriberBEDefinitionId, new List<string>() { "MSISDN", "ID" }, filterGroup);
            if (allSubscribers != null && allSubscribers.Count > 0)
            {
                foreach (var subscriber in allSubscribers)
                {
                    if (subscriber.FieldValues != null && subscriber.FieldValues.Count > 0)
                    {
                        var msisdnValue = subscriber.FieldValues.GetRecord("MSISDN");
                        if (msisdnValue != null && !subscribers.ContainsKey(msisdnValue.ToString()))
                        {
                            subscribers.Add(msisdnValue.ToString(), new SubscriberItem()
                            {
                                SubscriberId = (long)subscriber.FieldValues.GetRecord("ID"),
                                Type = (SubscriberType)subscriber.FieldValues.GetRecord("SubscriberType")
                            });
                        }
                    }
                }
            }
            return subscribers;
        }
    }
}
