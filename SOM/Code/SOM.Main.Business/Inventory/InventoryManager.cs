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
        public AktavaraConnector connector = new AktavaraConnector
        {
            BaseURL = "http://192.168.110.195:8901"
        };
        public InventoryPhoneItem GetInventoryPhoneItem(string phoneNumber)
        {
            InventoryPhoneItem result = null;
            
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
                    SWITCH_TYPE = detail.SWITCH_TYPE,
                    SwitchOMC = detail.SWITCH_OMC,
                    MDFPort = detail.MDF_PORT,
                    VerticalMDF = detail.MDF_VERT,
                    Receiver = detail.RECEIVER_NAME,
                    ReceiverPort = detail.RECEIVER_PORT,
                    DPPortId = detail.DP_PORT_ID,
                    DPId = detail.DP_ID,
                    SwitchId = detail.SWITCH_ID,
                    CabinetPrimaryPort = detail.PRIMARY_PORT,
                    CabinetSecondaryPort = detail.SECONDARY_PORT,
                    PATH_TYPE = detail.PATH_TYPE,
                    PhoneType = detail.PATH_TYPE.ToLower().Contains("wll") ? PhoneType.WLL : detail.PATH_TYPE == "PSTN_LINK" ? PhoneType.PSTN : PhoneType.ISDN
                };

            }

            return result;
        }
        public TechnicalReservationPhoneItem GetTechnicalReservation(string phoneNumber)
        {
            TechnicalReservationPhoneItem result = null;
            
            List<FreeReservationDetail> data = connector.Get<List<FreeReservationDetail>>("/FreeReservation/GET?number=" + phoneNumber + "&Override=1");

            if (data != null && data.Count > 0)
            {
                FreeReservationDetail detail = data[0];
                result = new TechnicalReservationPhoneItem
                {
                    Cabinet = detail.CABINET_NAME,
                    Transmitter = detail.WLL_TRANSMITTER_NAME,
                    TransmitterPort = detail.WLL_TRANSMITTER_PORT_NAME,
                    DP = detail.DP_NAME,
                    Switch = detail.SWITCH_NAME,
                    MDFPort = detail.MDF_PORT,
                    VerticalMDF = detail.MDF_VERT,
                    DPPortId = detail.DP_PORT_ID,
                    DPId = detail.DP_ID,
                    SwitchId = detail.SWITCH_ID,
                    TransmitterId = detail.WLL_TRANSMITTER_ID,
                    TransmitterPortId = detail.WLL_TRANSMITTER_PORT_ID,
                    TransmitterModuleId = detail.WLL_TRANSMITTER_MODULE_ID,
                    TransmitterModule = detail.WLL_TRANSMITTER_MODULE_NAME,
                    Threshold = detail.THRESHOLD,
                    SecondaryPortId = detail.SECONDARY_PORT_ID,
                    SecondaryPort = detail.SECONDARY_PORT,
                    ReceiverPortId = detail.WLL_RECEIVER_ID,
                    ReceiverId = detail.WLL_RECEIVER_ID,
                    Receiver = detail.WLL_RECEIVER_NAME,
                    ReceiverPort = detail.WLL_RECEIVER_PORT_NAME,
                    CabinetId = detail.CABINET_ID,
                    CanReserve = detail.CAN_RESERVE,
                    CurrentUtilization = detail.CURRENT_UTILIZATION,
                    DIDExist = detail.EXIST_DID,
                    DPMUXPort = detail.DP_MUX_PORT_NAME,
                    DPMUXPortId = detail.DP_MUX_PORT_ID,
                    DPPort = detail.DP_PORT,
                    ISDNExist = detail.EXIST_ISDN,
                    MDF = detail.MDF_NAME,
                    MDFId = detail.MDF_ID,
                    MDFPortId = detail.MDF_PORT_ID,
                    PrimaryMUXPort = detail.PRIMARY_MUX_PORT_NAME,
                    PrimaryMUXPortId = detail.PRIMARY_MUX_PORT_ID,
                    PrimaryPort = detail.PRIMARY_PORT,
                    PrimaryPortId = detail.PRIMARY_PORT_ID,
                    PSTNExist = detail.EXIST_PSTN,
                    VerticalMDFId = detail.MDF_VERT_ID

                };

            }

            return result;
        }
        public List<DPPortItem> GetFreePorts(string dpId)
        {
            List<DPPortItem> result = new List<DPPortItem>();
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
        public List<PortItem> GetCabinetPrimaryPorts(string cabinetId)
        {
            List<PortItem> result = new List<PortItem>();

            List<Port> data = connector.Get<List<Port>>("/TLDPrimaryPort/Get?id=" + cabinetId);

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    result.Add(new PortItem
                    {
                        Id = item.OBJECT_ID.ToString(),
                        Name = item.PORT
                    });
                }
            }

            return result;
        }
        public List<PortItem> GetCabinetSecondaryPorts(string cabinetId)
        {
            List<PortItem> result = new List<PortItem>();
            
            List<Port> data = connector.Get<List<Port>>("/TLDSecondaryPort/Get?id=" + cabinetId);

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    result.Add(new PortItem
                    {
                        Id = item.OBJECT_ID.ToString(),
                        Name = item.PORT
                    });
                }
            }

            return result;
        }
        public List<DPPortItem> GetDPPorts(string dpId)
        {
            List<DPPortItem> result = new List<DPPortItem>();
            
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
            List<PhoneNumberItem> phoneNumbers = new List<PhoneNumberItem>();
            List<AktavaraDevice> data = connector.Get<List<AktavaraDevice>>(string.Format("/FreeDevice/Get?switchid={0}&type={1}&top={2}", switchId, type, top));
            return data == null ? null : data.MapRecords(c => new DeviceItem
            {
                Id = c.OBJECT_ID,
                DeviceId = c.PORT
            }).ToList();
        }
        public ReserveLineRequestOutput ReservePhoneNumber(ReserveLineRequestInput input)
        {
            return new ReserveLineRequestOutput
            {
                Message = string.Format("Phone Number {0} is Reserved", input.PhoneNumber, input.PrimaryPort, input.SecondaryPort)
            };
        }
        public ReserveLineRequestOutput ReservePhoneNumber(string phoneNumber, string primaryPort, string secondaryPort)
        {
            return new ReserveLineRequestOutput
            {
                Message = string.Format("Phone Number {0} is Reserved", phoneNumber)
            };
        }

        //public TelephonyLineSubscriptionOutput InitiateTelephonyLineSubscriptionRequest(TelephonyLineSubscriptionInput input)
        //{

        //}
        public string GetTelephonyStatusDetails(string phoneNumber)
        {
            StringBuilder result = new StringBuilder();
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
        public List<MDFItemDetail> GetMDFLinkedToPrimary(string pPort)
        {
            List<MDFItemDetail> result = new List<MDFItemDetail>();
            List<MDFItem> data = connector.Get<List<MDFItem>>("/MDFlinkedtoprimary/Get?pport=" + pPort);

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    result.Add(new MDFItemDetail
                    {
                        Id = item.MDF_ID.ToString(),
                        Name = item.MDF,
                        Port = item.MDF_PORT.ToString(),
                        PortId = item.MDF_PORT_ID,
                        MdfVertical = item.MDF_VERTICAL.ToString(),
                        VerticalId = item.MDF_VERTICAL_ID
                    });
                }
            }
            return result;
        }
        public ReserveLineRequestOutput ReserveNumber(string pathType, string pathName, string objectList, string connectors)
        {
            ReserveLineRequestOutput result = new ReserveLineRequestOutput();
            ReserveLineRequestOutput data = connector.Get<ReserveLineRequestOutput>("/reservation/Get?PathType=" + pathType + "&PathName=" + pathName + "&Objectlist=" + objectList + "&connectors=" + connectors);

            if (data != null)
            {
                result.Message = string.Format("Phone Number is Reserved");
            }
            return result;
        }
        public string DeleteCPTReservation(string phoneNumber)
        {
            string result = "";
            string data = connector.Get<string>("/DeleteCPTReservation/POST?PhoneNumber=" + phoneNumber);

            if (data != null)
            {
                result = data;

            }
            return result.ToString();
        }
        public ReserveCPTRequestOutput ReserveCPT(ReserveCPTRequestInput input)
        {
            ReserveCPTRequestOutput result = new ReserveCPTRequestOutput();
            ReserveCPTRequestOutput data = connector.Get<ReserveCPTRequestOutput>("/ReserveCPT/POST?PhoneNumber=" + input.PhoneNumber + "&CPTID=" + input.CPTID);

            if (data != null)
            {
                result = data;

            }
            return result;
        }
        public CPTItem SearchCPT(string phoneNumber)
        {
            CPTItem result = new CPTItem();
            CPTItem data = connector.Get<CPTItem>("/SearchCPT/Get?Phonenumber=" + phoneNumber);

            if (data != null)
            {
                result = data;
            }
            return result;
        }
        public bool IsManualSwitch(string phoneNumber)
        {
            bool ismanual = false;
            List<TechnicalReservationDetail> data = connector.Get<List<TechnicalReservationDetail>>("/Techdetails/Get?id=" + phoneNumber);

            if (data != null && data.Count > 0)
            {
                TechnicalReservationDetail detail = data[0];
                ismanual = detail.SWITCH_TYPE == "Automatic" ? false : true;

            }

            return ismanual;
        }
        public string ReserveTelephonyNumber(string pathtype, string Pathname, string ObjectList, string connectors)
        {
            string result = "";
            string data = connector.Get<string>("/reservation/Get?pathtype=" + pathtype + "&Pathname=" + Pathname + "&ObjectList=" + ObjectList + "&connectors=" + connectors);

            if (data != null)
            {
                result = data;
            }
            return result;
        }
        public string ReserveNumber(string phoneNumber, string pathType, string phoneNumberID, string deviceID, string mDFPortID, string dPPortID, string primaryPort, string secondaryPort)
        {
            string result = "";
            string objectlist = phoneNumberID + "," + deviceID + "," + mDFPortID + "," + primaryPort + "," + secondaryPort + "," + dPPortID;
            string pathname = pathType + "_" + phoneNumber;
            string connectors = deviceID + "," + mDFPortID + ",COPPER_LINK|" + primaryPort + "," + secondaryPort + ",COPPER_LINK";

            string data = connector.Get<string>("/reservation/Get?pathtype=" + pathType + "&Pathname=" + pathname + "&ObjectList=" + objectlist + "&connectors=" + connectors);

            if (data != null)
            {
                result = data;
            }
            return result;
        }
        public List<PortItem> GetDSLAMPorts(string switchId)
        {
            List<PortItem> result = new List<PortItem>();
            List<DSLAMPort> data = connector.Get<List<DSLAMPort>>("/getdslamport/Get?switchid=" + switchId);

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    result.Add(new PortItem
                    {
                        Id = item.OBJECT_ID.ToString(),
                        Name = item.POS
                    });
                }
            }

            return result;
        }
        public LinePath CheckADSL(string phoneNumber)
        {

            LinePath result = new LinePath();
            List<PhoneLinePath> data = connector.Get<List<PhoneLinePath>>("/checkdsl/Get?phonenumber=" + phoneNumber);

            if (data != null && data.Count>0)
            {
                result.Path = data[0].PATH;
            }
            return result;
        }
        public List<ISPItem> GetISPs()
        {
            List<ISPItem> result = new List<ISPItem>();
            List<ISP> data = connector.Get<List<ISP>>("/GetISP/Get");

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    result.Add(new ISPItem
                    {
                        Id = item.FIELD_VALUE,
                        Name = item.FIELD_VALUE
                    });
                }
            }

            return result;
        }
        public List<PortItem> GetISPDSLAMPorts(string switchId,string ISP)
        {
            List<PortItem> result = new List<PortItem>();
            List<DSLAMPortItem> data = connector.Get<List<DSLAMPortItem>>("/getispdslamport/Get?ISP=" + ISP + "&switchid=" + switchId);

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    result.Add(new PortItem
                    {
                        Id = item.DSLAM_PORT_ID,
                        Name = item.DSLAM_PORT
                    });
                }
            }

            return result;
        }
        public string GetDeviceID(string phoneNumberID)
        {
            string result = "";
            List<DeviceIDDetail> data = connector.Get<List<DeviceIDDetail>>("/linkedobject/Get?id=" + phoneNumberID + "&type=device_id");
            if (data != null && data.Count > 0)
            {
                DeviceIDDetail detail = data[0];
                result = detail.SEC;
                
            }
            return result.ToString();
        }
        public InventoryPhoneItem GetTechnicalDetailsByPath(string pathID)
        {
            InventoryPhoneItem result = null;

            List<TechnicalReservationDetail> data = connector.Get<List<TechnicalReservationDetail>>("/Technicallinedetailsbypathid/Get?pathid=" + pathID);

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
                    SWITCH_TYPE = detail.SWITCH_TYPE,
                    SwitchOMC = detail.SWITCH_OMC,
                    MDFPort = detail.MDF_PORT,
                    VerticalMDF = detail.MDF_VERT,
                    Receiver = detail.RECEIVER_NAME,
                    ReceiverPort = detail.RECEIVER_PORT,
                    DPPortId = detail.DP_PORT_ID,
                    DPId = detail.DP_ID,
                    SwitchId = detail.SWITCH_ID,
                    CabinetPrimaryPort = detail.PRIMARY_PORT,
                    CabinetSecondaryPort = detail.SECONDARY_PORT,
                    PATH_TYPE = detail.PATH_TYPE,
                    PhoneType = detail.PATH_TYPE.ToLower().Contains("wll") ? PhoneType.WLL : detail.PATH_TYPE == "PSTN_LINK" ? PhoneType.PSTN : PhoneType.ISDN
                };

            }

            return result;
        }
        public List<DeviceDetailItem> GetDeviceIDs(string phoneNumbers)
        {
            List<DeviceDetailItem> result = new List<DeviceDetailItem>();
            List<DeviceDetailItem> data = connector.Get<List<DeviceDetailItem>>("/Deviceidsfromphonenumber/get?deviceids=" + phoneNumbers);

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    result.Add(new DeviceDetailItem
                    {
                        DEV_ID = item.DEV_ID,
                        SEC = item.SEC,
                        DEV_TYPE = item.DEV_TYPE,
                        PHONE_NUMBER = item.PHONE_NUMBER
                    });
                }
            }

            return result;
        }
        public string CreateFullPath(string phoneNumber, string pathID)
        {
            string result = "";
            result = connector.Get<string>("/createfullpath/get?pathid=" + pathID + "&phonenumber=" + phoneNumber);
            return result.ToString();
        }
    }

}
