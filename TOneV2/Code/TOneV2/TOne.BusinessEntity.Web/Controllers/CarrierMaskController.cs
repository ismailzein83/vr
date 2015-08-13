using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CarrierMaskController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public Object GetCarrierMasks(Vanrise.Entities.DataRetrievalInput<CarrierMaskQuery> input)
        {
            CarrierMaskManager manager = new CarrierMaskManager();
            return GetWebResponse(input, manager.GetCarrierMasks(input));
        }
    }
}