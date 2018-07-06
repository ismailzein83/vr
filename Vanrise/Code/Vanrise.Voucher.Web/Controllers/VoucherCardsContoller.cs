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
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "VoucherCards")]
    public class VoucherCardsController : BaseAPIController
    {
        VoucherCardsManager _voucherCardsManager = new VoucherCardsManager();

        [HttpGet]
        [Route("CheckVoucherAvailability")]
        public CheckVoucherAvailabilityOutput CheckVoucherAvailability(string pinCode, string lockedBy)
        {
            return _voucherCardsManager.CheckVoucherAvailability(pinCode, lockedBy);
        }

        [HttpPost]
        [Route("SetVoucherUsed")]
        public SetVoucherUsedOutput SetVoucherUsed(SetVoucherUsedInput input)
        {
            return _voucherCardsManager.SetVoucherUsed(input);
        }
        [HttpPost]
        [Route("ActivateVoucherCards")]
        public bool ActivateVoucherCards(VoucherCardsActivationInput voucherCardsActivationInput)
        {
            return _voucherCardsManager.ActivateVoucherCards(voucherCardsActivationInput);
        }

    }
}