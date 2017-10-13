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
        [HttpPost]
        [Route("GetFilteredSwitchReleaseCauses")]
        public object GetFilteredSwitchReleaseCauses(Vanrise.Entities.DataRetrievalInput<SwitchReleaseCauseQuery> input)
        {

            SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
            return GetWebResponse(input, manager.GetFilteredSwitchReleaseCauses(input));
        }
        [HttpPost]
        [Route("AddSwitchReleaseCause")]
        public InsertOperationOutput<SwitchReleaseCauseDetail> AddSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
            return manager.AddSwitchReleaseCause(switchReleaseCause);

        }
        [HttpGet]
        [Route("GetSwitchReleaseCause")]
        public SwitchReleaseCause GetSwitchReleaseCause(int switchReleaseCauseId)
        {
            SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
            return manager.GetSwitchReleaseCause(switchReleaseCauseId);



        }
        [HttpPost]
        [Route("UpdateSwitchReleaseCause")]
        public UpdateOperationOutput<SwitchReleaseCauseDetail> UpdateSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
            return manager.UpdateSwitchReleaseCause(switchReleaseCause);
        }

        [HttpGet]
        [Route("GetReleaseCausesByCode")]
        public List<SwitchReleaseCauseDetail> GetReleaseCausesByCode(string code, int? switchId = null)
        {
            SwitchReleaseCodeManager manager = new SwitchReleaseCodeManager();
            return manager.GetReleaseCausesByCode(code, switchId);
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
           public UploadSwitchReleaseCauseLog UploadSwitchReleaseCauses(int fileID,int switchId)
           {
           SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
           return manager.AddSwitchReleaseCauses(fileID, switchId);
         }
          [HttpGet]
          [Route("DownloadSwitchReleaseCauseLog")]
          public object DownloadCountryLog(long fileID)
          {
               SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
              byte[] bytes = manager.DownloadSwitchReleaseCauseLog(fileID);
              return GetExcelResponse(bytes, "ImportedSwitchReleaseCausesResults.xls");
          }
    }
}