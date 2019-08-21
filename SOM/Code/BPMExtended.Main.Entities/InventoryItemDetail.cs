﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public enum PhoneStatus { A, F, R }
    //public enum PhoneType { ISDN = 0, DID = 1, WLL = 2, PES = 3, Fiber = 4, PSTN = 5 }
    public class InventoryPhoneItemDetail
    {
        public string PhoneType { get; set; }
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
    }

    public class DPPortItemDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class PortItemDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class TechnicalReservationDetail
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

    public class GSHDSLTechnicalReservationDetail
    {
        public string Switch { get; set; }
        public string SwitchId { get; set; }
        public string SwitchType { get; set; }

        public string MDF { get; set; }
        public string MDFId { get; set; }      
        public string MDFPort { get; set; }
        public string MDFPortId { get; set; }

        public string Cabinet { get; set; }
        public string CabinetId { get; set; }
        public string PrimaryPort { get; set; }
        public string PrimaryPortId { get; set; }
        public string SecondaryPort { get; set; }
        public string SecondaryPortId { get; set; }

        public string DP { get; set; }
        public string DPId { get; set; }
        public string DPPort { get; set; }
        public string DPPortId { get; set; }


        public string CanReserve { get; set; }
        
        public string DSLAMPortId { get; set; }

    }



}
