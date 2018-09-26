using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;
using System.IO;


namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchReleaseCause")]
    public class SwitchReleaseCauseController : BaseAPIController
    {
        SwitchReleaseCauseManager _manager = new SwitchReleaseCauseManager();

        [HttpPost]
        [Route("GetFilteredSwitchReleaseCauses")]
        public object GetFilteredSwitchReleaseCauses(Vanrise.Entities.DataRetrievalInput<SwitchReleaseCauseQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredSwitchReleaseCauses(input), "Switch Release Causes");
        }
        [HttpPost]
        [Route("AddSwitchReleaseCause")]
        public InsertOperationOutput<SwitchReleaseCauseDetail> AddSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            return _manager.AddSwitchReleaseCause(switchReleaseCause);

        }
        [HttpGet]
        [Route("GetSwitchReleaseCause")]
        public SwitchReleaseCause GetSwitchReleaseCause(int switchReleaseCauseId)
        {
            return _manager.GetSwitchReleaseCause(switchReleaseCauseId);
        }
        [HttpPost]
        [Route("UpdateSwitchReleaseCause")]
        public UpdateOperationOutput<SwitchReleaseCauseDetail> UpdateSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            return _manager.UpdateSwitchReleaseCause(switchReleaseCause);
        }

        [HttpGet]
        [Route("GetReleaseCausesByCode")]
        public List<SwitchReleaseCauseDetail> GetReleaseCausesByCode(string code,[FromUri] List<int> switchIds)
        {
            return _manager.GetReleaseCauseDetailsByCode(code, switchIds);
        }
        [HttpGet]
        [Route("DownloadSwitchReleaseCausesTemplate")]
        public object DownloadSwitchReleaseCausesTemplate()
        {
            var template = "~/Client/Modules/WhS_BusinessEntity/Templates/ReleaseCauseTemplate.xlsx";
            string physicalPath = HttpContext.Current.Server.MapPath(template);
            byte[] bytes = File.ReadAllBytes(physicalPath);

            MemoryStream memStreamRate = new System.IO.MemoryStream();
            memStreamRate.Write(bytes, 0, bytes.Length);
            memStreamRate.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memStreamRate, "ReleaseCauseTemplate.xlsx");
        }
        [HttpGet]
        [Route("UploadSwitchReleaseCauses")]
        public UploadSwitchReleaseCauseLog UploadSwitchReleaseCauses(int fileID, int switchId)
        {
            return _manager.UploadSwitchReleaseCauses(fileID, switchId);
        }
        [HttpGet]
        [Route("DownloadSwitchReleaseCauseLog")]
        public object DownloadSwitchReleaseCauseLog(long fileID)
        {
            byte[] bytes = _manager.DownloadSwitchReleaseCauseLog(fileID);
            return GetExcelResponse(bytes, "ImportedSwitchReleaseCausesResults.xls");
        }

    }
}