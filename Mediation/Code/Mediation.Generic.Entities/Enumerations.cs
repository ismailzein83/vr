using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public enum CentrixSendCallType
    {
        Normal = 1,       
        Forward = 2,
        Transfer = 3,
        Pickup = 4,
        Routing = 5,
        Redirect = 6,
        Conference = 7,
        VoiceMail = 8,
        Cancel = 9,
        NoResponse = 10,
        Replaced = 11
    }

    public enum CentrixReceiveCallType
    {
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Normal)]
        Normal = 1,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Forward)]
        Forward = 2,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Transfer)]
        Transfer = 3,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Pickup)]
        Pickup = 4,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Routing)]
        Routing = 5,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Redirect)]
        Redirect = 6,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Conference)]
        Conference = 7,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.VoiceMail)]
        VoiceMail = 8,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Cancel)]
        Cancel = 9,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.NoResponse)]
        NoResponse = 10,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Replaced)]
        Replaced = 11
    }

    public class ReceiveCallTypeAttribute : Attribute
    {
        public CentrixSendCallType CorrespondingSendCallType { get; set; }
    }
}
