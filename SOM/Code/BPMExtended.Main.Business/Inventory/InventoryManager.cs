﻿using System;
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
                SWITCH_TYPE = item.SWITCH_TYPE,
                Transmitter = item.Transmitter,
                TransmitterPort = item.TransmitterPort,
                VerticalMDF = item.VerticalMDF,
                DPPortId = item.DPPortId,
                DPId = item.DPId

            };
        }

        public InventoryPhoneItemDetail GetGSHDSLInventoryDetailById(string contractId)
        {
            
            return new InventoryPhoneItemDetail
            {
                Cabinet = "c1",
                CabinetPrimaryPort = "99",
                CabinetSecondaryPort = "80",
                DP = "1",
                DPPorts = new List<string> { "8989" },
                DPSecondaryPorts = new List<string> { "9000" },
                DSlam = "d1",
                DSlamOMC = "o1",
                DSlamPort = "70",
                IsMultiplexed = false,
                MDFPort = "900",
                MSAN_EID = "4",
                MSAN_TID = "5",
                MSANType = "t1",
                Receiver = "e1",
                ReceiverPort = "98",
                Switch = "s1",
                SwitchId = "45",
                SwitchOMC = "m",
                SWITCH_TYPE = "Manual",
                Transmitter = "t4",
                TransmitterPort = "55",
                VerticalMDF = "v1",
                DPPortId = "900",
                DPId = "9999"

            };
        }

        public TechnicalReservationDetail GetTechnicalReservation(string phoneNumber)
        {
            TechnicalReservationPhoneItem item = null;
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<TechnicalReservationPhoneItem>(String.Format("api/SOM_Main/Inventory/GetTechnicalReservation?phoneNumber={0}", phoneNumber));
            }
            return new TechnicalReservationDetail
            {
                Switch = item.Switch,
                SwitchId = item.SwitchId,

                MDFPort = item.MDFPort,
                MDF = item.MDF,
                MDFPortId = item.MDFPortId,
                MDFId = item.MDFId,
                VerticalMDFId = item.VerticalMDFId,
                VerticalMDF = item.VerticalMDF,

                Cabinet = item.Cabinet,
                CabinetId = item.CabinetId,
                PrimaryPort = item.PrimaryPort,
                PrimaryPortId = item.PrimaryPortId,
                SecondaryPort = item.SecondaryPort,
                SecondaryPortId = item.SecondaryPortId,
                PrimaryMUXPort = item.PrimaryMUXPort,
                PrimaryMUXPortId = item.PrimaryMUXPortId,

                DP = item.DP,
                DPPortId = item.DPPortId,
                DPPort = item.DPPort,
                DPId = item.DPId,
                DPMUXPort = item.DPMUXPort,
                DPMUXPortId = item.DPMUXPortId,

                
                TransmitterId = item.TransmitterId,
                TransmitterModule = item.TransmitterModule,
                TransmitterModuleId = item.TransmitterModuleId,
                TransmitterPortId = item.TransmitterPortId,
                Transmitter = item.Transmitter,
                TransmitterPort = item.TransmitterPort,

                ReceiverId = item.ReceiverId,
                ReceiverPortId = item.ReceiverPortId,
                Receiver = item.Receiver,
                ReceiverPort = item.ReceiverPort,

                CanReserve = item.CanReserve,
                CurrentUtilization = item.CurrentUtilization,
                DIDExist = item.DIDExist,
                ISDNExist = item.ISDNExist,
                PSTNExist = item.PSTNExist,
                Threshold = item.Threshold
                
            };
        }

        public GSHDSLTechnicalReservationDetail GSHDSLGetTechnicalReservation(string phoneNumber)
        {
           
            return new GSHDSLTechnicalReservationDetail
            {
                Switch = "s1",
                SwitchId = "591.0",
                SwitchType ="Automatic",

                MDFPort = "mdf p1",
                MDF ="mdf1",
                MDFPortId = "mdf port id1",
                MDFId = "mdf id1",

                Cabinet = "c4469",
                CabinetId = "ci65",
                PrimaryPort = "pp65",
                PrimaryPortId = "we",
                SecondaryPort = "89",
                SecondaryPortId = "6891",

                DP = "65",
                DPPortId = "68767",
                DPPort = "43",
                DPId = "dp id1",

                DSLAMPortId = "900",

                CanReserve = "true",

            };
        }

        public GSHDSLTechnicalReservationDetail GSHDSLGetTechnicalReservationById(string contractId)
        {

            return new GSHDSLTechnicalReservationDetail
            {
                Switch = "s1",
                SwitchId = "591.0",
                SwitchType = "Automatic",

                MDFPort = "mdf p1",
                MDF = "mdf1",
                MDFPortId = "mdf port id1",
                MDFId = "mdf id1",

                Cabinet = "c4469",
                CabinetId = "ci65",
                PrimaryPort = "pp65",
                PrimaryPortId = "we",
                SecondaryPort = "89",
                SecondaryPortId = "6891",

                DP = "65",
                DPPortId = "68767",
                DPPort = "43",
                DPId = "dp id1",

                DSLAMPortId = "900",

                CanReserve = "true",

            };
        }

        public TechnicalReservationDetail GetTechnicalReservationByLinePathId(string linePathId)
        {

            return new TechnicalReservationDetail
            {
                Switch = "s1",
                SwitchId = "591.0",

                MDFPort = "mdf p1",
                MDF = "mdf1",
                MDFPortId = "mdf port id1",
                MDFId = "mdf id1",

                Cabinet = "c4469",
                CabinetId = "ci65",
                PrimaryPort = "pp65",
                PrimaryPortId = "we",
                SecondaryPort = "89",
                SecondaryPortId = "6891",

                DP = "65",
                DPPortId = "68767",
                DPPort = "43",
                DPId = "dp id1",

                DSLAMPortId = "900",

                CanReserve = "true",

            };
        }


        public bool changeCabinetPort(string portId)
        {
            //TODO: change cabinet port
            return true;
        }

        public bool changeDPPort(string portId)
        {
            //TODO: change DP port
            return true;
        }

        public DeportedNumberReservation GetDeportedNumberReservation(string phoneNumber)
        {
            return new DeportedNumberReservation
            {
                LinePathID = Guid.NewGuid().ToString(),
                MDF = "MDF001",
                MDFPort = "005",
                Cabinet = "Cabinet002",
                CabinetPort = "004",
                DP = "DP003",
                DPPort = "005"
            };
        }

        public bool ReserveDSLAMPort(string portId)
        {
            //TODO: reserve port 
            return true;


        }

        public bool MultiplexerValidation(string phoneNumber)
        {
            InventoryPhoneItemDetail inventoryItem = GetInventoryDetail(phoneNumber);

            return inventoryItem.IsMultiplexed ? true : false;

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

        public List<PortItemDetail> GetCabinetPrimaryPorts(string cabinetId)
        {
            List<PortItem> apiResult;
            using (SOMClient client = new SOMClient())
            {
                apiResult = client.Get<List<PortItem>>(String.Format("api/SOM_Main/Inventory/GetCabinetPrimaryPorts?cabinetId={0}", cabinetId));
            }

            return apiResult == null ? null : apiResult.MapRecords(r => new PortItemDetail
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }

        public List<PortItemDetail> GetCabinetSecondaryPorts(string cabinetId)
        {
            List<PortItem> apiResult;
            using (SOMClient client = new SOMClient())
            {
                apiResult = client.Get<List<PortItem>>(String.Format("api/SOM_Main/Inventory/GetCabinetSecondaryPorts?cabinetId={0}", cabinetId));
            }

            return apiResult == null ? null : apiResult.MapRecords(r => new PortItemDetail
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }

        public List<DPPortItemDetail> GetDPPorts(string dpPortId)
        {
            List<DPPortItem> apiResult;
            using (SOMClient client = new SOMClient())
            {
                apiResult = client.Get<List<DPPortItem>>(String.Format("api/SOM_Main/Inventory/GetDPPorts?dpId={0}", dpPortId));
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

        public bool IsNumbersOnSameSwitch(string contractId, string pilotContractId)
        {
            ContractManager contractManager = new ContractManager();
            TelephonyContractDetail contract = contractManager.GetTelephonyContract(contractId);
            TelephonyContractDetail pilotContract = contractManager.GetTelephonyContract(pilotContractId);

            InventoryPhoneItemDetail phoneitem = GetInventoryDetail(contract.PhoneNumber);
            InventoryPhoneItemDetail pilotPhoneItem = GetInventoryDetail(pilotContract.PhoneNumber);

            return phoneitem.SwitchId == pilotPhoneItem.SwitchId ? true : false;
        }

        public bool IsManualSwitch(string contractId)
        {

            ContractManager contractManager = new ContractManager();
            TelephonyContractDetail contract = contractManager.GetTelephonyContract(contractId);
            
            bool item = false;
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<bool>(String.Format("api/SOM_Main/Inventory/IsManualSwitch?phoneNumber={0}", contract.PhoneNumber));
            }
            return  item;
        }


        public bool IsManualDSLAM(string contractId)
        {

            ContractManager contractManager = new ContractManager();
            ADSLContractDetail contract = contractManager.GetADSLContract(contractId);

            bool item = false;
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<bool>(String.Format("api/SOM_Main/Inventory/IsManualSwitch?phoneNumber={0}", contract.PhoneNumber));
            }
            return item;
        }

        public bool IsManualDSLAMForGSHDSL(string contractId)
        {
            Random gen = new Random();
            int prob = gen.Next(10);
            return prob <= 5;

        }

        public string ReserveTelephonyNumber(string phoneNumber, string pathType, string phoneNumberID, string deviceID, string mDFPortID, string dPPortID, string primaryPort, string secondaryPort)
        {
            string objectlist = phoneNumberID + "," + deviceID + "," + mDFPortID + "," + primaryPort + "," + secondaryPort + "," + dPPortID;
            string pathname = pathType + "_" + phoneNumber;
            string connectors = deviceID + "," + mDFPortID + ",COPPER_LINK|" + primaryPort + "," + secondaryPort + ",COPPER_LINK";

            string  item = "";
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<string>(String.Format("api/SOM_Main/Inventory/ReserveTelephonyNumber?pathtype=" + pathType + "&Pathname=" + pathname + "&ObjectList=" + objectlist + "&connectors=" + connectors));
            }
            return item;
        }
        
        public List<ISPInfo> GetISPs()
        {
            List<ISPInfo> result = new List<ISPInfo>();
            List<ISPItem> phoneNumbers;
            using (SOMClient client = new SOMClient())
            {
                phoneNumbers = client.Get<List<ISPItem>>(String.Format("api/SOM_Main/Inventory/GetISPs"));
            }

            if (phoneNumbers != null)
            {
                foreach (var phoneNumber in phoneNumbers)
                {
                    result.Add(new ISPInfo
                    {
                        Id = phoneNumber.Id,
                        Name = phoneNumber.Name
                    });
                }
            }
            return result;

        }

    }
}
