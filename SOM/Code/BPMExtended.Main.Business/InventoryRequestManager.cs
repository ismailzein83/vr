using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using SOM.Main.Entities;

namespace BPMExtended.Main.Business
{
    public class InventoryRequestManager
    {
        public InventoryPhoneItemDetail GetInventoryDetail(string phoneNumber)
        {
            InventoryPhoneItem item = null;
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<InventoryPhoneItem>(String.Format("api/SOM_Main/Inventory/GetInventoryPhoneItem?phoneNumber={0}", phoneNumber));
            }
            return new InventoryPhoneItemDetail
            {
                Cabinet = item.Cabinet,
                CabinetPrimaryPort = item.CabinetPrimaryPort,
                CabinetSecondaryPort = item.CabinetSecondaryPort,
                DP = item.DP,
                DPPorts = item.DPPorts,
                DPSecondaryPorts = item.DPSecondaryPorts,
                DSlam = item.DSlam,
                DSlamOMC = item.DSlamOMC,
                DSlamPort = item.DSlamPort,
                IsMultiplexed = item.IsMultiplexed,
                MDFPort = item.MDFPort,
                MSAN_EID = item.MSAN_EID,
                MSAN_TID = item.MSAN_TID,
                MSANType = item.MSANType,
                PhoneStatus = (BPMExtended.Main.Entities.PhoneStatus)item.PhoneStatus,
                PhoneType = (BPMExtended.Main.Entities.PhoneType)item.PhoneType,
                Receiver = item.Receiver,
                ReceiverPort = item.ReceiverPort,
                Switch = item.Switch,
                SwitchId = item.SwitchId,
                SwitchOMC = item.SwitchOMC,
                Transmitter = item.Transmitter,
                TransmitterPort = item.TransmitterPort,
                VerticalMDF = item.VerticalMDF

            };
        }

        public List<PhoneNumberDetail> GetAvailablePhoneNumbers(string cabinetPort, string dpPort, bool isGold, bool isISDN, string startsWith)
        {
            List<PhoneNumberDetail> result = new List<PhoneNumberDetail>();
            List<PhoneNumber> phoneNumbers;
            using (SOMClient client = new SOMClient())
            {
                phoneNumbers = client.Get<List<PhoneNumber>>(String.Format("api/SOM_Main/Inventory/GetAvailableNumbers?cabinetPort={0}&dpPort={1}&isGold={2}&isISDN={3}&startsWith={4}", cabinetPort, dpPort, isGold, isISDN, startsWith));
            }

            if (phoneNumbers != null)
            {
                foreach (var phoneNumber in phoneNumbers)
                {
                    result.Add(new PhoneNumberDetail
                    {
                        IsGold = phoneNumber.IsGold,
                        IsISDN = phoneNumber.IsISDN,
                        Number = phoneNumber.Number
                    });
                }
            }
            return result;
        }

    }
}
