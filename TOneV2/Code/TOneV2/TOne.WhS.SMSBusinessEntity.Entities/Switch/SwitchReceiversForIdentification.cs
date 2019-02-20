using System;
using System;

namespace TOne.WhS.SMSBusinessEntity.Entities
{

    public enum SMSReceiver { Receiver = 0, CustomerReceiver = 1, SupplierReceiver = 2, MobileNetwork = 3 }

    public class SMSReceiversForIdentification
    {
        public string CustomerReceiver { get; set; }
        public string SupplierReceiver { get; set; }
        public string OutputReceiver { get; set; }
    }

    public class SMSReceiversForMobileNetworkMatch
    {
        public string MobileNetworkReceiver { get; set; }
    }
}
