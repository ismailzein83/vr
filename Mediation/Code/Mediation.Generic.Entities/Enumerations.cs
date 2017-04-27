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
        Conference = 2,
        Forward = 3,
        Transfer = 4,
        Pickup = 5,
        Routing = 6,
        Redirect = 7,
        VoiceMail = 8,
        Cancel = 9
    }

    public enum CentrixReceiveCallType
    {
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Normal)]
        Normal = 1,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Conference)]
        Conference = 2,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Forward)]
        Forward = 3,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Transfer)]
        Transfer = 4,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Pickup)]
        Pickup = 5,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Routing)]
        Routing = 6,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Redirect)]
        Redirect = 7,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.VoiceMail)]
        VoiceMail = 8,
        [ReceiveCallTypeAttribute(CorrespondingSendCallType = CentrixSendCallType.Cancel)]
        Cancel = 9
    }

    public class ReceiveCallTypeAttribute : Attribute
    {
        public CentrixSendCallType CorrespondingSendCallType { get; set; }
    }
}
