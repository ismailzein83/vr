﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public enum PhoneStatus { A, F, R }
    public enum PhoneType { ISDN = 0, DID = 1, WLL = 2, PES = 3, Fiber = 4 }
    public class InventoryPhoneItem
    {

        public PhoneType PhoneType { get; set; }
        public PhoneStatus PhoneStatus { get; set; }
        public string VerticalMDF { get; set; }
        public string MDFPort { get; set; }
        public string Cabinet { get; set; }
        public string CabinetPrimaryPort { get; set; }
        public string CabinetSecondaryPort { get; set; }
        public string DP { get; set; }
        public List<string> DPPort { get; set; }
        public string Switch { get; set; }
        public string SwitchId { get; set; }
        public string SwitchOMC { get; set; }


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


        public bool IsMultiplexed { get; set; }

    }
    public class PhoneNumber
    {
        public string Number { get; set; }
        public bool IsGold { get; set; }
        public bool IsISDN { get; set; }
    }
}
