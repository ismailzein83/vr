using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.CDRProcessing.Business;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.CDRProcessing.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "DefineCDRFields")]
    [JSONWithTypeAttribute]
    public class DefineCDRFieldsController:BaseAPIController
    {
         [HttpPost]
         [Route("GetFilteredCDRFields")]
         public object GetFilteredCDRFields(Vanrise.Entities.DataRetrievalInput<CDRFieldsQuery> input)
         {
             DefineCDRFieldsManager manager = new DefineCDRFieldsManager();
             return GetWebResponse(input, manager.GetFilteredCDRFields(input));
         }

         [HttpPost]
         [Route("UpdateCDRField")]
         public TOne.Entities.UpdateOperationOutput<CDRFieldDetail> UpdateCDRField(CDRField cdrField)
         {
             DefineCDRFieldsManager manager = new DefineCDRFieldsManager();
            return manager.UpdateCDRField(cdrField);
         }

         [HttpPost]
         [Route("AddCDRField")]
         public TOne.Entities.InsertOperationOutput<CDRFieldDetail> AddCDRField(CDRField cdrField)
         {
             DefineCDRFieldsManager manager = new DefineCDRFieldsManager();
            return manager.AddCDRField(cdrField);
         }

         [HttpGet]
         [Route("GetCDRFieldTypeTemplates")]
         public List<Vanrise.Entities.TemplateConfig> GetCDRFieldTypeTemplates()
         {
             DefineCDRFieldsManager manager = new DefineCDRFieldsManager();
             return manager.GetCDRFieldTypeTemplates();
         }

         [HttpGet]
         [Route("DeleteCDRField")]
         public TOne.Entities.DeleteOperationOutput<CDRFieldDetail> DeleteCDRField(int cdrFieldId)
         {
             DefineCDRFieldsManager manager = new DefineCDRFieldsManager();
             return manager.DeleteCDRField(cdrFieldId);
         }
         [HttpGet]
         [Route("GetCDRField")]
         public CDRField GetCDRField(int cdrFieldId)
         {
             DefineCDRFieldsManager manager = new DefineCDRFieldsManager();
             return manager.GetCDRField(cdrFieldId);
         }
    }
}