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
   [RoutePrefix(Constants.ROUTE_PREFIX + "NumberPrefix")]
    [JSONWithTypeAttribute]
    public class NumberPrefixController:BaseAPIController
    {
         [HttpPost]
         [Route("GetFilteredNumberPrefixes")]
       public object GetFilteredNumberPrefixes(Vanrise.Entities.DataRetrievalInput<NumberPrefixesQuery> input)
         {
             NumberPrefixesManager manager = new NumberPrefixesManager();
             return GetWebResponse(input, manager.GetFilteredNumberPrefixes(input));
         }

         [HttpPost]
         [Route("UpdateNumberPrefix")]
         public Vanrise.Entities.UpdateOperationOutput<NumberPrefixDetail> UpdateNumberPrefix(NumberPrefix fixedPrefix)
         {
             NumberPrefixesManager manager = new NumberPrefixesManager();
             return manager.UpdateNumberPrefix(fixedPrefix);
         }

         [HttpPost]
         [Route("AddNumberPrefix")]
         public Vanrise.Entities.InsertOperationOutput<NumberPrefixDetail> AddNumberPrefix(NumberPrefix fixedPrefix)
         {
             NumberPrefixesManager manager = new NumberPrefixesManager();
            return manager.AddNumberPrefix(fixedPrefix);
         }

         [HttpGet]
         [Route("DeleteNumberPrefix")]
         public Vanrise.Entities.DeleteOperationOutput<NumberPrefixDetail> DeleteNumberPrefix(int fixedPrefixId)
         {
             NumberPrefixesManager manager = new NumberPrefixesManager();
             return manager.DeleteNumberPrefix(fixedPrefixId);
         }
         [HttpGet]
         [Route("GetNumberPrefix")]
         public NumberPrefix GetNumberPrefix(int fixedPrefixId)
         {
             NumberPrefixesManager manager = new NumberPrefixesManager();
             return manager.GetNumberPrefix(fixedPrefixId);
         }


         [HttpGet]
         [Route("GetPrefixesInfo")]
         public IEnumerable<NumberPrefixInfo> GetPrefixesInfo()
         {
             NumberPrefixesManager manager = new NumberPrefixesManager();
             return manager.GetPrefixesInfo();
         }


    }
}