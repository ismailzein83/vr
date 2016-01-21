using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "DefineFixedPrefixes")]
    [JSONWithTypeAttribute]
    public class DefineFixedPrefixesController:BaseAPIController
    {
         [HttpPost]
         [Route("GetFilteredFixedPrefixes")]
       public object GetFilteredFixedPrefixes(Vanrise.Entities.DataRetrievalInput<FixedPrefixesQuery> input)
         {
             DefineFixedPrefixesManager manager = new DefineFixedPrefixesManager();
             return GetWebResponse(input, manager.GetFilteredFixedPrefixes(input));
         }

         [HttpPost]
         [Route("UpdateFixedPrefix")]
         public Vanrise.Entities.UpdateOperationOutput<FixedPrefixDetail> UpdateFixedPrefix(FixedPrefix fixedPrefix)
         {
             DefineFixedPrefixesManager manager = new DefineFixedPrefixesManager();
             return manager.UpdateFixedPrefix(fixedPrefix);
         }

         [HttpPost]
         [Route("AddFixedPrefix")]
         public Vanrise.Entities.InsertOperationOutput<FixedPrefixDetail> AddFixedPrefix(FixedPrefix fixedPrefix)
         {
             DefineFixedPrefixesManager manager = new DefineFixedPrefixesManager();
            return manager.AddFixedPrefix(fixedPrefix);
         }

         [HttpGet]
         [Route("DeleteFixedPrefix")]
         public Vanrise.Entities.DeleteOperationOutput<FixedPrefixDetail> DeleteFixedPrefix(int fixedPrefixId)
         {
             DefineFixedPrefixesManager manager = new DefineFixedPrefixesManager();
             return manager.DeleteFixedPrefix(fixedPrefixId);
         }
         [HttpGet]
         [Route("GetFixedPrefix")]
         public FixedPrefix GetFixedPrefix(int fixedPrefixId)
         {
             DefineFixedPrefixesManager manager = new DefineFixedPrefixesManager();
             return manager.GetFixedPrefix(fixedPrefixId);
         }


         [HttpGet]
         [Route("GetPrefixesInfo")]
         public IEnumerable<FixedPrefixInfo> GetPrefixesInfo()
         {
             DefineFixedPrefixesManager manager = new DefineFixedPrefixesManager();
             return manager.GetPrefixesInfo();
         }


    }
}