﻿using System;
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
        public TechnicalReservationPhoneItem GetTechnicalReservation(string phoneNumber)
        {
            TechnicalReservationPhoneItem result = null;

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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
        public List<PortItem> GetCabinetPrimaryPorts(string cabinetId)
        {
            List<PortItem> result = new List<PortItem>();

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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
                DeviceId = c.PORT
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
        public List<MDFItemDetail> GetMDFLinkedToPrimary(string pPort)
        {
            List<MDFItemDetail> result = new List<MDFItemDetail>();

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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
            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            CPTItem data = connector.Get<CPTItem>("/SearchCPT/Get?Phonenumber=" + phoneNumber);

            if (data != null)
            {
                result = data;
            }
            return result;
        }

        public bool IsManualSwitch(string phoneNumber)
        {
            
            //AktavaraConnector connector = new AktavaraConnector
            //{
            //    BaseURL = "http://192.168.110.195:8901"
            //};
            //CPTItem data = connector.Get<CPTItem>("/SearchCPT/Get?Phonenumber=" + phoneNumber);

            Random gen = new Random();
            int prob = gen.Next(100);
            return prob <= 50;
        }
        public string ReserveTelephonyNumber(string pathtype, string Pathname, string ObjectList, string connectors)
        {
            string result = "";

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };

            string data = connector.Get<string>("/reservation/Get?pathtype=" + pathtype + "&Pathname=" + Pathname + "&ObjectList=" + ObjectList + "&connectors=" + connectors);

            if (data != null)
            {
                result = data;
            }
            return result;
        }
        public List<PortItem> GetDSLAMPorts(string switchId)
        {
            List<PortItem> result = new List<PortItem>();

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
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
    }

}
