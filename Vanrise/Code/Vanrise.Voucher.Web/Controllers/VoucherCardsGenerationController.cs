using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.GenericData.Entities;
using Vanrise.Voucher.Business;
using Vanrise.Voucher.Web;
using Vanrise.Voucher.Entities;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "VoucherCardsGeneration")]
    public class VoucherCardsGenerationController : BaseAPIController
    {
        VoucherCardsGenerationsManager _manager = new VoucherCardsGenerationsManager();

        
        [HttpGet]
        [Route("GetVoucherCardsGeneration")]
        public VoucherCardsGeneration GetVoucherCardsGeneration(long voucherCardsGenerationId)
        {

            return _manager.GetVoucherCardsGeneration(voucherCardsGenerationId);
        }

  
  
       
         
    }
}