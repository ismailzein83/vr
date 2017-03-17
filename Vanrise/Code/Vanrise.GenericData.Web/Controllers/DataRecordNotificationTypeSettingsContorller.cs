//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http;
//using Vanrise.GenericData.Entities;
//using Vanrise.GenericData.Notification;
//using Vanrise.Web.Base;

//namespace Vanrise.GenericData.Web.Controllers
//{
//    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordNotificationTypeSettings")]
//    [JSONWithTypeAttribute]
//    public class DataRecordNotificationTypeSettingsController : BaseAPIController
//    {
//        DataRecordNotificationTypeSettingsManager _manager = new DataRecordNotificationTypeSettingsManager();

//        [HttpGet]
//        [Route("GetNotificationGridColumnAttributes")]
//        public List<DataRecordGridColumnAttribute> GetNotificationGridColumnAttributes(Guid notificationTypeId)
//        {
//            return _manager.GetNotificationGridColumnAttributes(notificationTypeId);
//        }
//    }
//}