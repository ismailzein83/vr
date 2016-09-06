using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Pop")]
    public class PopController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredPops")]
        public object GetFilteredPops(Vanrise.Entities.DataRetrievalInput<PopQuery> input)
        {
            PopManager manager = new PopManager();
            return GetWebResponse(input, manager.GetFilteredPops(input));
        }
        
        [HttpGet]
        [Route("GetPop")]
        public Pop GetPop(int popId)
        {
            PopManager manager = new PopManager();
            return manager.GetPop(popId);
        }

        [HttpPost]
        [Route("AddPop")]
        public Vanrise.Entities.InsertOperationOutput<Pop> AddPop(Pop pop)
        {

            PopManager manager = new PopManager();
            return manager.AddPop(pop);
        }

        [HttpPost]
        [Route("UpdatePop")]
        public Vanrise.Entities.UpdateOperationOutput<Pop> UpdatePop(Pop pop)
        {
            PopManager manager = new PopManager();
            return manager.UpdatePop(pop);
        }
       
    }
}