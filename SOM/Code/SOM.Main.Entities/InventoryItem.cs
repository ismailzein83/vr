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
                DPPort = new List<string> { "DPPort1", "DPPort2" },
                DSlam = "DSlam1",
                DSlamPort = "DSlamPort1",
                SwitchOMC = "SwitchOMC1",
                PhoneStatus = PhoneStatus.A

            });

            result.Add("223344", new InventoryPhoneItem
            {
                Cabinet = "C2",
                CabinetPrimaryPort = "CabinetPrimaryPort_222",
                PhoneType = PhoneType.PES,
                PhoneStatus = Entities.PhoneStatus.F,
                CabinetSecondaryPort = "CabinetSecondaryPort_333",
                DP = "DP2",
                DPPort = new List<string> { "DPPort5", "DPPort3" },
                DSlam = "DSlam2",
                DSlamPort = "DSlamPort2",
                MDFPort = "MDFPort2",
                MSAN_EID = "MSAN_EID1",
                MSAN_TID = "MSAN_TID1",
                MSANType = "POTS",
                IsMultiplexed = true
            });

            result.Add("556677", new InventoryPhoneItem
            {
                Cabinet = "C5",
                CabinetPrimaryPort = "CabinetPrimaryPort_444",
                PhoneType = PhoneType.WLL,
                CabinetSecondaryPort = "CabinetSecondaryPort_555",
                DP = "DP6",
                PhoneStatus = Entities.PhoneStatus.F,
                DPPort = new List<string> { "DPPort15", "DPPort22" },
                DSlam = "DSlam2",
                DSlamPort = "DSlamPort2",
                DSlamOMC = "DSlamOMC1",
                MDFPort = "MDFPort2",
                Transmitter = "Transmitter1",
                TransmitterPort = "TransmitterPort1",
                Receiver = "Receiver1",
                ReceiverPort = "ReceiverPort1",
                IsMultiplexed = true
            });

            return result;
        }

        public static Dictionary<string, List<PhoneNumber>> GetMockPhoneNumbers()
        {
            Dictionary<string, List<PhoneNumber>> result = new Dictionary<string, List<PhoneNumber>>();

            result.Add("CabinetPrimaryPort_444_DPPort15", new List<PhoneNumber> { new PhoneNumber { IsGold = true, IsISDN = false, Number = "555522" }, new PhoneNumber { IsGold = false, IsISDN = true, Number = "4655645" }, new PhoneNumber { IsGold = true, IsISDN = true, Number = "998877" } });
            result.Add("CabinetPrimaryPort_444_DPPort22", new List<PhoneNumber> { new PhoneNumber { IsGold = true, IsISDN = false, Number = "888777" }, new PhoneNumber { IsGold = false, IsISDN = true, Number = "8745451" }, new PhoneNumber { IsGold = true, IsISDN = true, Number = "555666" } });

            result.Add("CabinetPrimaryPort_222_DPPort5", new List<PhoneNumber> { new PhoneNumber { IsGold = true, IsISDN = false, Number = "111333" }, new PhoneNumber { IsGold = false, IsISDN = true, Number = "8954541" }, new PhoneNumber { IsGold = true, IsISDN = true, Number = "123456" } });
            result.Add("CabinetPrimaryPort_222_DPPort3", new List<PhoneNumber> { new PhoneNumber { IsGold = true, IsISDN = false, Number = "444999" }, new PhoneNumber { IsGold = false, IsISDN = true, Number = "1215745" }, new PhoneNumber { IsGold = true, IsISDN = true, Number = "987654" } });

            result.Add("CabinetPrimaryPort_123_DPPort1", new List<PhoneNumber> { new PhoneNumber { IsGold = true, IsISDN = false, Number = "887766" }, new PhoneNumber { IsGold = false, IsISDN = true, Number = "2154541" }, new PhoneNumber { IsGold = true, IsISDN = true, Number = "200500" } });
            result.Add("CabinetPrimaryPort_123_DPPort2", new List<PhoneNumber> { new PhoneNumber { IsGold = true, IsISDN = false, Number = "100200" }, new PhoneNumber { IsGold = false, IsISDN = true, Number = "9865524" }, new PhoneNumber { IsGold = true, IsISDN = true, Number = "600200" } });


            return result;
        }

    }

    #endregion

}
