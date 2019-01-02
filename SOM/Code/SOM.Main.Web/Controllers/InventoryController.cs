using System;
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
        [Route("GetTechnicalDetails")]
        public InventoryPhoneItem GetTechnicalDetails(string phoneNumber)
        {
            // change  object type from 'InventoryPhoneItem' class to 'TechnicalDetails'
            InventoryManager manager = new InventoryManager();
            return manager.GetInventoryPhoneItem(phoneNumber);
        }
        [HttpGet]
        [Route("GetTemporaryTechnicalReservation")]
        public TechnicalReservationPhoneItem GetTemporaryTechnicalReservation(string phoneNumber)
        {
            // change  object type from 'TechnicalReservationPhoneItem' class to 'TechnicalReservation'
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
        public ReserveLineRequestOutput ReservePhoneNumber(string phoneNumber , string primaryPort , string secondaryPort)
        {
            InventoryManager manager = new InventoryManager();
            return manager.ReservePhoneNumber(phoneNumber, primaryPort, secondaryPort);
        }
        [HttpGet]
        [Route("GetFreeDevices")]
        public List<DeviceItem> GetFreeDevices(string switchId, string lineType, int top)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetDevices(switchId, lineType, top);
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
        [Route("GetFreeDPPorts")]
        public List<DPPortItem> GetFreeDPPorts(string dpId)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetFreePorts(dpId);
        }

        [HttpGet]
        [Route("ReserveNumber")]
        public string ReserveNumber(string phoneNumber, string pathType, string phoneNumberID, string deviceID, string mDFPortID, string dPPortID, string primaryPort, string secondaryPort)
        {
            InventoryManager manager = new InventoryManager();
            return manager.ReserveNumber(phoneNumber, pathType, phoneNumberID, deviceID,mDFPortID,dPPortID,primaryPort,secondaryPort);
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
            // change  object type from 'LinePath' class to 'ADSLLinePath'
            InventoryManager manager = new InventoryManager();
            return manager.CheckADSL(phoneNumber);
        }
        [HttpGet]
        [Route("GetISPs")]
        public List<ISPItem> GetISPs()
        {
            // change  object type from 'ISPItem' class to 'ISP'
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
        [HttpGet]
        [Route("GetDeviceID")]
        public string GetDeviceID(string phoneNumberID)
        {
            // change  object type from 'ISPItem' class to 'ISP'
            InventoryManager manager = new InventoryManager();
            return manager.GetDeviceID(phoneNumberID);
        }
        [HttpGet]
        [Route("GetTechnicalDetailsByPath")]
        public InventoryPhoneItem GetTechnicalDetailsByPath(string pathID)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetTechnicalDetailsByPath(pathID);
        }
        [HttpGet]
        [Route("GetDeviceIDs")]
        public List<DeviceDetailItem> GetDeviceIDs(string phoneNumbers)
        {
            // change  object type from 'DeviceDetailItem' class to 'Device'
            InventoryManager manager = new InventoryManager();
            return manager.GetDeviceIDs(phoneNumbers);
        }
        [HttpGet]
        [Route("CreateFullPath")]
        public string CreateFullPath(string phoneNumber,string pathID)
        {
            InventoryManager manager = new InventoryManager();
            return manager.CreateFullPath(phoneNumber,pathID);
        }


        #endregion


        [HttpGet]
        [Route("GetDPPorts")]
        public List<DPPortItem> GetDPPorts(string dpId)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetDPPorts(dpId);
        }
        [HttpGet]
        [Route("GetInventoryPhoneItem")]
        public InventoryPhoneItem GetInventoryPhoneItem(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetInventoryPhoneItem(phoneNumber);
        }
        [HttpGet]
        [Route("ReserveTelephonyNumber")]
        public string ReserveTelephonyNumber(string pathtype, string Pathname, string ObjectList, string connectors)
        {
            InventoryManager manager = new InventoryManager();
            return manager.ReserveTelephonyNumber(pathtype, Pathname, ObjectList, connectors);
        }

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