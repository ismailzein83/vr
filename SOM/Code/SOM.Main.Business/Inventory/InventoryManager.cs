using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOM.Main.Entities;
using Vanrise.Common;

namespace SOM.Main.Business
{
    public class InventoryManager
    {

        public InventoryPhoneItem GetInventoryPhoneItem(string phoneNumber)
        {
            InventoryPhoneItem result = null;

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            List<TechnicalReservationDetail> data = connector.Get<List<TechnicalReservationDetail>>("/Techdetails/Get?id=" + phoneNumber);

            if (data != null && data.Count > 0)
            {
                TechnicalReservationDetail detail = data[0];
                result = new InventoryPhoneItem
                {
                    Cabinet = detail.CABINET_NAME,
                    Transmitter = detail.TRANSMITTER_NAME,
                    TransmitterPort = detail.TRANSMITTER_PORT,
                    DP = detail.DP_NAME,
                    Switch = detail.SWITCH_NAME,
                    SwitchOMC = detail.SWITCH_OMC,
                    MDFPort = detail.MDF_PORT,
                    VerticalMDF = detail.MDF_VERT,
                    Receiver = detail.RECEIVER_NAME,
                    ReceiverPort = detail.RECEIVER_PORT,
                    DPPortId = detail.DP_PORT_ID,
                    DPId = detail.DP_ID,
                    SwitchId = detail.SWITCH_ID

                };

            }

            return result;
        }

        public List<DPPortItem> GetFreePorts(string dpId)
        {
            List<DPPortItem> result = new List<DPPortItem>();

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            List<DPPort> data = connector.Get<List<DPPort>>("/FreeDPPorts/Get?id=" + dpId);

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    result.Add(new DPPortItem
                    {
                        Id = item.DP_PORT_ID.ToString(),
                        Name = item.DP_PORT_NAME
                    });
                }
            }

            return result;
        }

        public List<PhoneNumberItem> GetAvailableNumbers(string cabinetPort, string dpPort, bool isGold, bool isISDN, string startsWith)
        {
            List<PhoneNumberItem> phoneNumbers = new List<PhoneNumberItem>();
            GenerateInvtoryMockData.GetMockPhoneNumbers().TryGetValue(string.Format("{0}_{1}", cabinetPort, dpPort), out phoneNumbers);
            return phoneNumbers == null ? null : phoneNumbers.Where(p => p.IsGold == isGold && p.IsISDN == isISDN && (string.IsNullOrEmpty(startsWith) || p.Number.StartsWith(startsWith))).ToList();
        }

        public List<PhoneNumberItem> GetAvailableNumbers(string switchId, string category, string type, int top)
        {
            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            List<PhoneNumberItem> phoneNumbers = new List<PhoneNumberItem>();
            List<AktavaraPhoneNumber> data = connector.Get<List<AktavaraPhoneNumber>>(string.Format("/FreeNumbers/Get?switchid={0}&category={1}&type={2}&top={3}", switchId, category, type, top));
            return data == null ? null : data.MapRecords(c => new PhoneNumberItem
            {
                Id = c.OBJECT_ID,
                Number = c.PHONE_NUMBER
            }).ToList();
        }

        public List<DeviceItem> GetDevices(string switchId, string type, int top)
        {
            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            List<PhoneNumberItem> phoneNumbers = new List<PhoneNumberItem>();
            List<AktavaraDevice> data = connector.Get<List<AktavaraDevice>>(string.Format("/FreeDevice/Get?switchid={0}&type={1}&top={2}", switchId, type, top));
            return data == null ? null : data.MapRecords(c => new DeviceItem
            {
                Id = c.OBJECT_ID,
                DeviceId = c.DEVICE_ID
            }).ToList();
        }

        public ReserveLineRequestOutput ReservePhoneNumber(ReserveLineRequestInput input)
        {
            return new ReserveLineRequestOutput
            {
                Message = string.Format("Phone Number {0} is Reserved", input.PhoneNumber)
            };
        }

        //public TelephonyLineSubscriptionOutput InitiateTelephonyLineSubscriptionRequest(TelephonyLineSubscriptionInput input)
        //{

        //}

        public string GetTelephonyStatusDetails(string phoneNumber)
        {
            StringBuilder result = new StringBuilder();
            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            List<AktavaraPhoneStatusDetail> data = connector.Get<List<AktavaraPhoneStatusDetail>>("/ComplaintTechDetails/Get?number=" + phoneNumber);

            if (data != null && data.Count > 0)
            {
                AktavaraPhoneStatusDetail detail = data[0];

                result.AppendLine("PHONE NUMBER = " + detail.POHNE_NUMBER);
                result.AppendLine("Status = " + detail.PHONE_STATUS);
                result.AppendLine("CABINET = " + detail.CABINET_NAME);
                result.AppendLine("DEVICE = " + detail.DEV_NAME);

            }
            return result.ToString();
        }

        public string TestPhoneLine(string phoneNumber)
        {
            StringBuilder result = new StringBuilder();
            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            List<AktavaraPhoneStatusDetail> data = connector.Get<List<AktavaraPhoneStatusDetail>>("/ComplaintTechDetails/Get?number=" + phoneNumber);

            if (data != null && data.Count > 0)
            {
                AktavaraPhoneStatusDetail detail = data[0];
                result.AppendLine("Status = " + detail.PHONE_STATUS);
                result.AppendLine("MDF VERT = " + detail.MDF_VERT);
                result.AppendLine("MDF PORT = " + detail.MDF_PORT);
                result.AppendLine("Status = " + detail.MDF_PORT_STATUS);
                result.AppendLine("FREE DP PORTS = " + detail.NUM_FREE_DP_PORTS);
                result.AppendLine("FREE SECONDARY DP PORTS = " + detail.NUM_FREE_SECONDARY_PORTS);
            }
            return result.ToString();
        }
    }

}
