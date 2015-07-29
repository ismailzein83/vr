using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.Entities
{
    public class CDRMainBatch : PersistentQueueItem
    {
        static CDRMainBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRMainBatch),"MainCDRs");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingCDRMain), "cost", "sale", "ID", "Attempt", "Alert", "Connect",
                    "Disconnect", "DurationInSeconds", "CDPN", "CGPN", "Port_OUT", "Port_IN", "ReleaseCode",
                    "ReleaseSource", "SwitchID", "SwitchCdrID", "Tag", "OurZoneID", "SupplierZoneID", "OriginatingZoneID",
                    "SIP", "Extra_Fields", "CustomerID", "SupplierID", "OurCode", "SupplierCode", "IsValid", "IsRerouted", "CDPNOut", "SubscriberID");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingCDRSale), "DurationInSeconds", "ID", "ZoneID", "Net", "CurrencySymbol", "RateValue",
                    "RateID", "Discount", "RateType", "ToDConsiderationID", "FirstPeriod", "RepeatFirstperiod", "FractionUnit",
                    "TariffID", "CommissionValue", "CommissionID", "ExtraChargeValue", "ExtraChargeID", "Updated", "Attempt","BillingCDRMainID", "Code");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingCDRCost), "DurationInSeconds", "ID", "ZoneID", "Net", "CurrencySymbol", "RateValue",
                                                            "RateID", "Discount", "RateType", "ToDConsiderationID", "FirstPeriod", "RepeatFirstperiod", "FractionUnit",
                                                            "TariffID", "CommissionValue", "CommissionID", "ExtraChargeValue", "ExtraChargeID", "Updated", "Attempt","BillingCDRMainID", "Code");
        }


        public List<BillingCDRMain> MainCDRs { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} Main Billing CDRs", MainCDRs.Count);
        }
    }
}