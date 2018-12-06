﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SOM.Main.Business;
using SOM.Main.Entities;
using Vanrise.Web.Base;

namespace SOM.Main.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Inventory")]
    public class InventoryController : BaseAPIController
    {
        #region needed 
        [HttpGet]
        [Route("GetInventoryPhoneItem")]
        public InventoryPhoneItem GetInventoryPhoneItem(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetInventoryPhoneItem(phoneNumber);
        }
        [HttpGet]
        [Route("GetTechnicalReservation")]
        public TechnicalReservationPhoneItem GetTechnicalReservation(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetTechnicalReservation(phoneNumber);
        }
        [HttpGet]
        [Route("GetAvailableNumbers")]
        public List<PhoneNumberItem> GetAvailableNumbers(string switchId, string category, string type, int top)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetAvailableNumbers(switchId, category, type, top);
        }
        [HttpPost]
        [Route("ReservePhoneNumber")]
        public ReserveLineRequestOutput ReservePhoneNumber(ReserveLineRequestInput input)
        {
            InventoryManager manager = new InventoryManager();
            return manager.ReservePhoneNumber(input);
        }
        [HttpGet]
        [Route("GetDevices")]
        public List<DeviceItem> GetDevices(string switchId, string type, int top)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetDevices(switchId, type, top);
        }
        [HttpGet]
        [Route("ReserveCPT")]
        public ReserveCPTRequestOutput ReserveCPT(string phoneNumber, string cptId)
        {
            InventoryManager manager = new InventoryManager();
            ReserveCPTRequestInput input = new ReserveCPTRequestInput()
            {
                PhoneNumber = phoneNumber,
                CPTID = cptId
            };
            return manager.ReserveCPT(input);
        }
        [HttpGet]
        [Route("DeleteCPTReservation")]
        public string DeleteCPTReservation(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.DeleteCPTReservation(phoneNumber);
        }
        [HttpGet]
        [Route("IsManualSwitch")]
        public bool IsManualSwitch(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.IsManualSwitch(phoneNumber);
        }
        [HttpGet]
        [Route("GetCabinetPrimaryPorts")]
        public List<PortItem> GetCabinetPrimaryPorts(string cabinetId)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetCabinetPrimaryPorts(cabinetId);
        }
        [HttpGet]
        [Route("GetCabinetSecondaryPorts")]
        public List<PortItem> GetCabinetSecondaryPorts(string cabinetId)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetCabinetSecondaryPorts(cabinetId);
        }
        [HttpGet]
        [Route("GetFreePorts")]
        public List<DPPortItem> GetFreePorts(string dpPortId)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetFreePorts(dpPortId);
        }
        [HttpGet]
        [Route("GetDPPorts")]
        public List<DPPortItem> GetDPPorts(string dpId)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetDPPorts(dpId);
        }
        [HttpGet]
        [Route("ReserveTelephonyNumber")]
        public string ReserveTelephonyNumber(string pathtype, string Pathname, string ObjectList, string connectors)
        {
            InventoryManager manager = new InventoryManager();
            return manager.ReserveTelephonyNumber(pathtype, Pathname, ObjectList, connectors);
        }
        [HttpGet]
        [Route("GetDSLAMPorts")]
        public List<PortItem> GetDSLAMPorts(string switchId)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetDSLAMPorts(switchId);
        }
        [HttpGet]
        [Route("CheckADSL")]
        public LinePath CheckADSL(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.CheckADSL(phoneNumber);
        }
        [HttpGet]
        [Route("GetISPs")]
        public List<ISPItem> GetISPs()
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetISPs();
        }
        [HttpGet]
        [Route("GetISPDSLAMPorts")]
        public List<PortItem> GetISPDSLAMPorts(string switchId, string ISP)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetISPDSLAMPorts(switchId, ISP);
        }
        #endregion





        [HttpGet]
        [Route("GetAvailableNumbers")]
        public List<PhoneNumberItem> GetAvailableNumbers(string cabinetPort, string dpPort, bool isGold, bool isISDN, string startsWith)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetAvailableNumbers(cabinetPort, dpPort, isGold, isISDN, startsWith);
        }
                       
        [HttpPost]
        [Route("InitiateTelephonyLineSubscriptionRequest")]
        public TelephonyLineSubscriptionOutput InitiateTelephonyLineSubscriptionRequest(TelephonyLineSubscriptionInput input)
        {
            throw new NotImplementedException();
            //InventoryManager manager = new InventoryManager();
            //return manager.InitiateTelephonyLineSubscriptionRequest(input);
        }

        [HttpGet]
        [Route("GetTelephonyStatusDetails")]
        public string GetTelephonyStatusDetails(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetTelephonyStatusDetails(phoneNumber);
        }

        [HttpGet]
        [Route("TestPhoneLine")]
        public string TestPhoneLine(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.TestPhoneLine(phoneNumber);
        }
        
        
    }
}