using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Tenants")]
    public class TenantsController : Vanrise.Web.Base.BaseAPIController
    {
        private TenantManager _manager = new TenantManager();
        public TenantsController()
        {
            this._manager = new TenantManager();
        }

        [HttpPost]
        [Route("GetFilteredTenants")]
        public object GetFilteredTenants(Vanrise.Entities.DataRetrievalInput<TenantQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredTenants(input), "Tenants");
        }

        [HttpGet]
        [Route("GetTenantsInfo")]
        public IEnumerable<TenantInfo> GetTenantsInfo(string filter = null)
        {
            TenantFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<TenantFilter>(filter) : null;
            return _manager.GetTenantsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetTenantbyId")]
        public Tenant GetTenantbyId(int tenantId)
        {
            return _manager.GetTenantbyId(tenantId);
        }

        [HttpPost]
        [Route("UpdateTenant")]
        public Vanrise.Entities.UpdateOperationOutput<TenantDetail> UpdateTenant(Tenant tenantObject)
        {
            return _manager.UpdateTenant(tenantObject);
        }

        [HttpPost]
        [Route("AddTenant")]
        public Vanrise.Entities.InsertOperationOutput<TenantDetail> AddTenant(Tenant tenantObject)
        {
            return _manager.AddTenant(tenantObject);
        }
    }
}