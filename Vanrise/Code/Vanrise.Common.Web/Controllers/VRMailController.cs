using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRMail")]
    [JSONWithTypeAttribute]
    public class VRMailController : BaseAPIController
    {
        [HttpPost]
        [Route("SendTestEmail")]
        public void SendTestEmail(EmailSettingDetail setting)
        {
            VRMailManager vrMailManager = new VRMailManager();
            vrMailManager.SendTestMail(setting.EmailSettingData, setting.FromEmail, setting.ToEmail, setting.Subject, setting.Body);
        }
        [HttpPost]
        [Route("SendEmail")]
        public void SendEmail(VRMail emailSetting)
        {
            var vrMailManager = new VRMailManager();
            var vrFileManager = new VRFileManager();
            var vrMailAttachements = emailSetting.AttachementFileIds.Select(fileId => vrMailManager.ConvertToGeneralAttachement(fileId));
            vrMailManager.SendMail(emailSetting.From,emailSetting.To, emailSetting.CC, emailSetting.BCC, emailSetting.Subject, emailSetting.Body
                , vrMailAttachements.ToList(), emailSetting.CompressFile);
        }
        [HttpGet]
        [Route("GetFileName")]
        public string GetFileName(long fileId)
        {
            var vrFileManager = new VRFileManager();
            var file = vrFileManager.GetFile(fileId);
            return file != null ? file.Name : string.Empty;
        }
        [HttpGet]
        [Route("DownloadAttachement")]
        public object DownloadAttachement(long fileId)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileId);
            return GetExcelResponse(file.Content, file.Name);
        }
    }
}
