﻿using Retail.NIM.Business;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.NIM.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "IMSPhoneNumber")]
    public class IMSPhoneNumberController : BaseAPIController
    {
        IMSPhoneNumberManager manager = new IMSPhoneNumberManager();

        [HttpPost]
        [Route("GetFreeIMSPhoneNumbers")]
        public GetFreeIMSPhoneNumbersOutput GetFreeIMSPhoneNumbers(GetFreeIMSPhoneNumbersInput input)
        {
            return manager.GetFreeIMSPhoneNumbers(input);
        }

        [HttpPost]
        [Route("ReserveIMSPhoneNumber")]
        public ReserveIMSPhoneNumberOutput ReserveIMSPhoneNumber(ReserveIMSPhoneNumberInput input)
        {
            return manager.ReserveIMSPhoneNumber(input);
        }

        [HttpPost]
        [Route("SetUsedIMSPhoneNumber")]
        public SetUsedIMSPhoneNumberOutput SetUsedIMSPhoneNumber(SetUsedIMSPhoneNumberInput input)
        {
            return manager.SetUsedIMSPhoneNumber(input);
        }
    }
}