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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRVoucherCards")]
    public class VRVoucherCardsContoller : BaseAPIController
    {
        VoucherCardsManager _voucherCardsManager = new VoucherCardsManager();

        [HttpGet]
        [Route("CheckVoucherAvailability")]
        public CheckVoucherAvailabilityOutput CheckVoucherAvailability(string pinCode)
        {
            return _voucherCardsManager.CheckVoucherAvailability(pinCode);
        }

         [HttpPut]
        [Route("SetVoucherUsed")]
        public SetVoucherUsedOutput SetVoucherUsed(SetVoucherUsedInput input)
        {
            return _voucherCardsManager.SetVoucherUsed(input);
        }
    }
}