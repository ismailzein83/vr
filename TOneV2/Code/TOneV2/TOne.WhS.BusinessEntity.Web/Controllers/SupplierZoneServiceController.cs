using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierZoneService")]
    public class SupplierZoneServiceController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSupplierZoneServices")]
        public object GetFilteredSupplierZoneServices(Vanrise.Entities.DataRetrievalInput<SupplierZoneServiceQuery> input)
        {
            SupplierZoneServiceManager manager = new SupplierZoneServiceManager();
            return GetWebResponse(input, manager.GetFilteredSupplierZoneServices(input), "Supplier Zone Services");
        }

        [HttpPost]
        [Route("UpdateSupplierZoneService")]
        public UpdateOperationOutput<SupplierEntityServiceDetail> UpdateSupplierZoneService(SupplierZoneServiceToEdit serviceObject)
        {
            SupplierZoneServiceManager manager = new SupplierZoneServiceManager();
            return manager.UpdateSupplierZoneService(serviceObject);
        }
    }

}