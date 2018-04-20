using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    #region Mock Data
    public class GenerateInvtoryMockData
    {

        public static Dictionary<string, InventoryPhoneItem> GetMockInventoryPhoneItem()
        {
            Dictionary<string, InventoryPhoneItem> result = new Dictionary<string, InventoryPhoneItem>();

            result.Add("112233", new InventoryPhoneItem
            {
                VerticalMDF = "VMDF1",
                MDFPort = "MDFPort1",
                Cabinet = "C1",
                CabinetPrimaryPort = "CabinetPrimaryPort_123",
                PhoneType = PhoneType.ISDN,
                CabinetSecondaryPort = "CabinetSecondaryPort_124",
                DP = "DP1",
                DPPorts = new List<string> { "DPPort1", "DPPort2" },
                DPSecondaryPorts = new List<string> { "DPSecondaryPorts1", "DPSecondaryPorts3", "DPSecondaryPorts5" },
                DSlam = "DSlam1",
                DSlamPort = "DSlamPort1",
                SwitchOMC = "SwitchOMC1",
                PhoneStatus = PhoneStatus.A,
                Switch = "Midan",
                SwitchId = "SW_MD"

            });

            result.Add("223344", new InventoryPhoneItem
            {
                Cabinet = "C2",
                CabinetPrimaryPort = "CabinetPrimaryPort_222",
                PhoneType = PhoneType.PES,
                PhoneStatus = Entities.PhoneStatus.F,
                CabinetSecondaryPort = "CabinetSecondaryPort_333",
                DP = "DP2",
                DPPorts = new List<string> { "DPPort5", "DPPort3" },
                DPSecondaryPorts = new List<string> { "DPSecondaryPorts11", "DPSecondaryPorts4", "DPSecondaryPorts35" },
                DSlam = "DSlam2",
                DSlamPort = "DSlamPort2",
                MDFPort = "MDFPort2",
                MSAN_EID = "MSAN_EID1",
                MSAN_TID = "MSAN_TID1",
                MSANType = "POTS",
                IsMultiplexed = true,
                Switch = "Mazze",
                SwitchId = "SW_MZ"
            });

            result.Add("556677", new InventoryPhoneItem
            {
                Cabinet = "C5",
                CabinetPrimaryPort = "CabinetPrimaryPort_444",
                PhoneType = PhoneType.WLL,
                CabinetSecondaryPort = "CabinetSecondaryPort_555",
                DP = "DP6",
                PhoneStatus = Entities.PhoneStatus.F,
                DPPorts = new List<string> { "DPPort15", "DPPort22" },
                DPSecondaryPorts = new List<string> { "DPSecondaryPorts10", "DPSecondaryPorts54" },
                DSlam = "DSlam2",
                DSlamPort = "DSlamPort2",
                DSlamOMC = "DSlamOMC1",
                MDFPort = "MDFPort2",
                Transmitter = "Transmitter1",
                TransmitterPort = "TransmitterPort1",
                Receiver = "Receiver1",
                ReceiverPort = "ReceiverPort1",
                IsMultiplexed = true,
                Switch = "Khaldiye",
                SwitchId = "SW_KH"
            });

            return result;
        }

        public static Dictionary<string, List<PhoneNumberItem>> GetMockPhoneNumbers()
        {
            Dictionary<string, List<PhoneNumberItem>> result = new Dictionary<string, List<PhoneNumberItem>>();

            result.Add("DPPort15_DPSecondaryPorts10", new List<PhoneNumberItem> { new PhoneNumberItem { IsGold = true, IsISDN = false, Number = "555522" }, new PhoneNumberItem { IsGold = false, IsISDN = true, Number = "4655645" }, new PhoneNumberItem { IsGold = true, IsISDN = true, Number = "998877" } });
            result.Add("DPPort22_DPSecondaryPorts54", new List<PhoneNumberItem> { new PhoneNumberItem { IsGold = true, IsISDN = false, Number = "888777" }, new PhoneNumberItem { IsGold = false, IsISDN = true, Number = "8745451" }, new PhoneNumberItem { IsGold = true, IsISDN = true, Number = "555666" } });

            result.Add("DPPort5_DPSecondaryPorts11", new List<PhoneNumberItem> { new PhoneNumberItem { IsGold = true, IsISDN = false, Number = "111333" }, new PhoneNumberItem { IsGold = false, IsISDN = true, Number = "8954541" }, new PhoneNumberItem { IsGold = true, IsISDN = true, Number = "123456" } });
            result.Add("DPPort3_DPSecondaryPorts4", new List<PhoneNumberItem> { new PhoneNumberItem { IsGold = true, IsISDN = false, Number = "444999" }, new PhoneNumberItem { IsGold = false, IsISDN = true, Number = "1215745" }, new PhoneNumberItem { IsGold = true, IsISDN = true, Number = "987654" } });

            result.Add("DPPort1_DPSecondaryPorts1", new List<PhoneNumberItem> { new PhoneNumberItem { IsGold = true, IsISDN = false, Number = "887766" }, new PhoneNumberItem { IsGold = false, IsISDN = true, Number = "2154541" }, new PhoneNumberItem { IsGold = true, IsISDN = true, Number = "200500" } });
            result.Add("DPPort2_DPSecondaryPorts5", new List<PhoneNumberItem> { new PhoneNumberItem { IsGold = true, IsISDN = false, Number = "100200" }, new PhoneNumberItem { IsGold = false, IsISDN = true, Number = "9865524" }, new PhoneNumberItem { IsGold = true, IsISDN = true, Number = "600200" } });


            return result;
        }

    }

    #endregion

}
