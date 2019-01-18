using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class TechnicalReservation
    {
        public string Switch { get; set; }
        public string SwitchId { get; set; }

        public string MDF { get; set; }
        public string MDFId { get; set; }
        public string VerticalMDF { get; set; }
        public string VerticalMDFId { get; set; }
        public string MDFPort { get; set; }
        public string MDFPortId { get; set; }

        public string Cabinet { get; set; }
        public string CabinetId { get; set; }
        public string PrimaryPort { get; set; }
        public string PrimaryPortId { get; set; }
        public string PrimaryMUXPort { get; set; }
        public string PrimaryMUXPortId { get; set; }
        public string SecondaryPort { get; set; }
        public string SecondaryPortId { get; set; }

        public string DP { get; set; }
        public string DPId { get; set; }
        public string DPPort { get; set; }
        public string DPPortId { get; set; }
        public string DPMUXPort { get; set; }
        public string DPMUXPortId { get; set; }

        public string Transmitter { get; set; }
        public string TransmitterId { get; set; }
        public string TransmitterPort { get; set; }
        public string TransmitterPortId { get; set; }
        public string TransmitterModule { get; set; }
        public string TransmitterModuleId { get; set; }
        public string Receiver { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverPort { get; set; }
        public string ReceiverPortId { get; set; }

        public string PSTNExist { get; set; }
        public string ISDNExist { get; set; }
        public string DIDExist { get; set; }
        public string CanReserve { get; set; }
        public string CurrentUtilization { get; set; }
        public string Threshold { get; set; }

        public string DSLAMPortId { get; set; }
    }
}
