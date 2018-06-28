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

        [Route("CheckAvailablePinCode")]
        public VoucherCardResult CheckAvailablePinCode(string pinCode)
        {
            return _voucherCardsManager.CheckAvailablePinCode(pinCode);
        }
        
        [Route("SetVoucherUsed")]
        public VoucherCardResult SetVoucherUsed(string pinCode, string usedBy)
        {
            return _voucherCardsManager.SetVoucherUsed(pinCode, usedBy);
        }
    }
}