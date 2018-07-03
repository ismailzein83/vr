using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.Ringo.ProxyAPI
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Topup")]
    public class TopupController:BaseAPIController
    {
        TopupManager _topupManager = new TopupManager();
        [HttpPost]
        [Route("AddTopup")]
        public AddTopupOutput AddTopup(AddTopupInput input)
        {
            return _topupManager.AddTopup(input);
        }
        [IsAnonymous]
        [HttpPost]
        [Route("AddTopupAnonymous")]
        public AddTopupOutput AddTopupAnonymous(AddTopupInput input)
        {
            return _topupManager.AddTopup(input);
        }
    }
}
