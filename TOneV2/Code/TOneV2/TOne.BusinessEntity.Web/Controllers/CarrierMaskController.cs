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
        public Object GetFilteredCarrierMasks(Vanrise.Entities.DataRetrievalInput<CarrierMaskQuery> input)
        {
            CarrierMaskManager manager = new CarrierMaskManager();
            return GetWebResponse(input, manager.GetFilteredCarrierMasks(input));
        }

        [HttpGet]
        public CarrierMask GetCarrierMask(int carrierMaskId)
        {
            CarrierMaskManager manager = new CarrierMaskManager();
            return manager.GetCarrierMask(carrierMaskId);
        }

        [HttpPost]
        public TOne.Entities.UpdateOperationOutput<CarrierMask> UpdateCarrierMask(CarrierMask carrierMask)
        {
            CarrierMaskManager manager = new CarrierMaskManager();
            return manager.UpdateCarrierMask(carrierMask);
        }

        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<CarrierMask> AddCarrierMask(CarrierMask carrierMask)
        {
            CarrierMaskManager manager = new CarrierMaskManager();
            return manager.AddCarrierMask(carrierMask);
        }
    }
}