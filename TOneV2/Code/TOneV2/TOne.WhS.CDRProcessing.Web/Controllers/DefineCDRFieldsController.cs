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
         public void UpdateCDRField(CDRField cdrField)
         {
             DefineCDRFieldsManager manager = new DefineCDRFieldsManager();
            // manager.UpdateCDRField(cdrFields);
         }
         [HttpPost]
         [Route("AddCDRField")]
         public void AddCDRField(CDRField cdrField)
         {
             DefineCDRFieldsManager manager = new DefineCDRFieldsManager();
             manager.AddCDRField(cdrField);
         }
    }
}