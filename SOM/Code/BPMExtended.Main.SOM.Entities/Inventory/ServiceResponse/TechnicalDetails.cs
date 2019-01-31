using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public enum PhoneType { ISDN = 0, DID = 1, WLL = 2, PES = 3, Fiber = 4, PSTN = 5 }
    public enum PhoneStatus { A, F, R }
    public class TechnicalDetails
    {
        public PhoneType PhoneType { get; set; }
        public PhoneStatus PhoneStatus { get; set; }
        public string VerticalMDF { get; set; }
        public string MDFPort { get; set; }
        public string Cabinet { get; set; }
        public string CabinetPrimaryPort { get; set; }
        public string CabinetSecondaryPort { get; set; }
        public string DP { get; set; }
        public List<string> DPPorts { get; set; }
        public List<string> DPSecondaryPorts { get; set; }
        public string Switch { get; set; }
        public string SwitchId { get; set; }
        public string SwitchOMC { get; set; }
        public string SWITCH_TYPE { get; set; }


        public string DSlam { get; set; }
        public string DSlamPort { get; set; }
        public string DSlamOMC { get; set; }


        public string Transmitter { get; set; }
        public string TransmitterPort { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPort { get; set; }


        public string MSAN_EID { get; set; }
        public string MSAN_TID { get; set; }
        public string MSANType { get; set; }
        public string DPId { get; set; }
        public string DPPortId { get; set; }
        public bool IsMultiplexed { get; set; }

        public string SwitchStatus { get; set; }
        public string MDFStatus { get; set; }
        public string CabinetStatus { get; set; }
        public string DPStatus { get; set; }

        public string IsSwitchFaulty { get; set; }
        public string IsMDFFaulty { get; set; }
        public string IsCabinetFaulty { get; set; }
        public string IsDPFaulty { get; set; }
        public string MDF { get; set; }



    }
}
