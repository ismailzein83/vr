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
        
         [HttpGet]
         [Route("GetPrefixes")]
         public IEnumerable<NumberPrefix> GetPrefixes()
         {
             NumberPrefixesManager manager = new NumberPrefixesManager();
             return manager.GetPrefixes();
         }


         [HttpPost]
         [Route("UpdatePrefixes")]
         public bool UpdatePrefixes(List<NumberPrefix> prefixes)
         {
             NumberPrefixesManager manager = new NumberPrefixesManager();
             return manager.UpdatePrefixes(prefixes);
         }


    }
}