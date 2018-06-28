using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Voucher.Business;
using Vanrise.Voucher.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Voucher.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VoucherCardDefinition")]
    public class VoucherCardDefinitionController : BaseAPIController
    {
        VoucherCardDefinitionManager _VoucherCardDefinitionManager = new VoucherCardDefinitionManager();
        VoucherCardsGenerationsManager _VoucherCardsGenerationsManager = new VoucherCardsGenerationsManager();
        [HttpGet]
        [Route("GetVoucherCardDefinition")]
        public List<VoucharCardSerialNumberPart> GetVoucherCardDefinition()
        {
            return _VoucherCardDefinitionManager.GetVoucherCardDefinition();
        }
    }
}