using Retail.NIM.Business;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.NIM.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "IMSPhoneNumber")]
    public class IMSPhoneNumberController : BaseAPIController
    {
        IMSPhoneNumberManager manager = new IMSPhoneNumberManager();

        [HttpGet]
        [Route("GetFreeIMSPhoneNumbers")]
        public GetFreeIMSPhoneNumbersOutput GetFreeIMSPhoneNumbers(string imsNumber, int? category)
        {
            return manager.GetFreeIMSPhoneNumbers(new GetFreeIMSPhoneNumbersInput
            {
                IMSNumber = imsNumber,
                Category = category
            });
        }

        [HttpPost]
        [Route("ReserveIMSPhoneNumber")]
        public ReserveIMSPhoneNumberOutput ReserveIMSPhoneNumber(ReserveIMSPhoneNumberInput input)
        {
            return manager.ReserveIMSPhoneNumber(input);
        }
    }
}