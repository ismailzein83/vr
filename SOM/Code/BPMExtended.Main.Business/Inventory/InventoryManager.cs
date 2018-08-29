using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using SOM.Main.BP.Arguments;
using SOM.Main.Entities;

namespace BPMExtended.Main.Business
{
    public class InventoryManager
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
                VerticalMDF = item.VerticalMDF,
                DPPortId = item.DPPortId,
                DPId = item.DPId

            };
        }

        public List<DPPortItemDetail> GetFreePorts(string dpPortId)
        {
            List<DPPortItem> apiResult;
            using (SOMClient client = new SOMClient())
            {
                apiResult = client.Get<List<DPPortItem>>(String.Format("api/SOM_Main/Inventory/GetFreePorts?dpPortId={0}", dpPortId));
            }

            return apiResult == null ? null : apiResult.MapRecords(r => new DPPortItemDetail
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }

        public List<PhoneNumberDetail> GetAvailablePhoneNumbers(string cabinetPort, string dpPort, bool isGold, bool isISDN, string startsWith)
        {
            List<PhoneNumberDetail> result = new List<PhoneNumberDetail>();
            List<PhoneNumberItem> phoneNumbers;
            using (SOMClient client = new SOMClient())
            {
                phoneNumbers = client.Get<List<PhoneNumberItem>>(String.Format("api/SOM_Main/Inventory/GetAvailableNumbers?cabinetPort={0}&dpPort={1}&isGold={2}&isISDN={3}&startsWith={4}", cabinetPort, dpPort, isGold, isISDN, startsWith));
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

        public List<PhoneNumberDetail> GetAvailablePhoneNumbers(string switchId, string category, string type, int top)
        {
            List<PhoneNumberDetail> result = new List<PhoneNumberDetail>();
            List<PhoneNumberItem> phoneNumbers;
            using (SOMClient client = new SOMClient())
            {
                phoneNumbers = client.Get<List<PhoneNumberItem>>(String.Format("api/SOM_Main/Inventory/GetAvailableNumbers?switchId={0}&category={1}&type={2}&top={3}", switchId, category, type, top));
            }

            if (phoneNumbers != null)
            {
                foreach (var phoneNumber in phoneNumbers)
                {
                    result.Add(new PhoneNumberDetail
                    {
                        IsGold = phoneNumber.IsGold,
                        IsISDN = phoneNumber.IsISDN,
                        Number = phoneNumber.Number,
                        Id = phoneNumber.Id
                    });
                }
            }
            return result;
        }

        public List<DeviceItemDetail> GetDevices(string switchId, string type, int top)
        {
            List<DeviceItemDetail> result = new List<DeviceItemDetail>();
            List<DeviceItem> deviceItems;
            using (SOMClient client = new SOMClient())
            {
                deviceItems = client.Get<List<DeviceItem>>(String.Format("api/SOM_Main/Inventory/GetDevices?switchId={0}&type={1}&top={2}", switchId, type, top));
            }

            if (deviceItems != null)
            {
                foreach (var deviceItem in deviceItems)
                {
                    result.Add(new DeviceItemDetail
                    {
                        Id = deviceItem.Id,
                        DeviceId = deviceItem.DeviceId
                    });
                }
            }
            return result;
        }

        public ReserveLineRequestOutput ReservePhoneNumber(BPMCustomerType customerType, Guid accountOrContactId, ReserveLineRequestInput reserveLineInput)
        {
            ReserveLineRequestOutput output = null;

            using (var client = new SOMClient())
            {
                output = client.Post<ReserveLineRequestInput, ReserveLineRequestOutput>("api/SOM_Main/Inventory/ReservePhoneNumber", reserveLineInput);
            }
            return output;
        }

        public CreateCustomerRequestOutput InitiateTelephonyLineSubscriptionRequest(BPMCustomerType customerType, Guid accountOrContactId, TelephonyLineSubscriptionSomRequestSetting telephonyLineSubscriptionSomRequestSetting)
        {
            string title = string.Format("Telephony Line Subscription '{0}'", telephonyLineSubscriptionSomRequestSetting.PhoneNumber);

            return Helper.CreateSOMRequest(customerType, accountOrContactId, title, telephonyLineSubscriptionSomRequestSetting);
        }

        public string TestPhoneLine(string phoneNumber)
        {
            string result = null;

            using (SOMClient client = new SOMClient())
            {
                result = client.Get<string>(String.Format("api/SOM_Main/Inventory/TestPhoneLine?phoneNumber={0}", phoneNumber));
            }

            return result;
        }

        public string GetTelephonyStatusDetails(string phoneNumber)
        {
            string result = null;

            using (SOMClient client = new SOMClient())
            {
                result = client.Get<string>(String.Format("api/SOM_Main/Inventory/GetTelephonyStatusDetails?phoneNumber={0}", phoneNumber));
            }

            return result;
        }
    }
}
