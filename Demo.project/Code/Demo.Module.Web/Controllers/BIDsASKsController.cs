using Demo.Module.Business;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
	[RoutePrefix(Constants.ROUTE_PREFIX + "BIDsASKs")]
	[JSONWithTypeAttribute]
	public class BIDsASKsController : BaseAPIController
	{
		BIDsASKsManager bidsASKsManager = new BIDsASKsManager();

		[HttpPost]
		[Route("GetFilteredBIDs")]
		public object GetFilteredBIDs(DataRetrievalInput<BIDsQuery> input)
		{
			return GetWebResponse(input, bidsASKsManager.GetFilteredBIDs(input));
		}

        [HttpPost]
        [Route("GetFilteredASKs")]
        public object GetFilteredASKs(DataRetrievalInput<ASKsQuery> input)
        {
            return GetWebResponse(input, bidsASKsManager.GetFilteredASKs(input));
        }

    }
}