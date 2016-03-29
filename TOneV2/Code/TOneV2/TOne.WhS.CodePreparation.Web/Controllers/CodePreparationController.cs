using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.CodePreparation.Business;
using Vanrise.Web.Base;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodePreparation")]
    public class CodePreparationController : BaseAPIController
    {
        [HttpGet]
        [Route("DownloadImportCodePreparationTemplate")]
        public object DownloadImportCodePreparationTemplate()
        {
            CodePreparationManager manager = new CodePreparationManager();
            byte[] bytes = manager.DownloadImportCodePreparationTemplate();
            return GetExcelResponse(bytes, "Code Preparation Template.xls");  
        }

        [HttpGet]
        [Route("GetChanges")]
        public Changes GetChanges(int sellingNumberPlanId)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.GetChanges(sellingNumberPlanId);
        }


        [HttpPost]
        [Route("SaveNewZone")]
        public NewZoneOutput SaveNewZone(NewZoneInput input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.SaveNewZone(input);
        }

        [HttpPost]
        [Route("SaveNewCode")]
        public NewCodeOutput SaveNewCode(NewCodeInput input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.SaveNewCode(input);
        }

        [HttpPost]
        [Route("MoveCodes")]
        public MoveCodeOutput MoveCodes(MoveCodeInput input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.MoveCodes(input);
        }

        [HttpPost]
        [Route("CloseCodes")]
        public CloseCodesOutput CloseCodes(CloseCodesInput input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.CloseCodes(input);
        }

        [HttpGet]
        [Route("GetZoneItems")]
        public List<ZoneItem> GetZoneItems(int sellingNumberPlanId, int countryId)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.GetZoneItems(sellingNumberPlanId, countryId);
        }

        [HttpGet]
        [Route("CheckCodePreparationState")]
        public bool CheckCodePreparationState(int sellingNumberPlanId)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.CheckCodePreparationState(sellingNumberPlanId);
        }
        [HttpGet]
        [Route("CancelCodePreparationState")]
        public bool CancelCodePreparationState(int sellingNumberPlanId)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.CancelCodePreparationState(sellingNumberPlanId);
        }
        [HttpPost]
        [Route("GetCodeItems")]
        public object GetCodeItems(Vanrise.Entities.DataRetrievalInput<GetCodeItemInput> input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return GetWebResponse(input, manager.GetCodeItems(input));
        }


    }
}