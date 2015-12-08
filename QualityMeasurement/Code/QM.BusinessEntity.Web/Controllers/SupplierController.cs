using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using QM.BusinessEntity.Web;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace QM.BusinessEntity.Web.Controllers
{
   
    [RoutePrefix(Constants.ROUTE_PREFIX + "Supplier")]

    [JSONWithTypeAttribute]
    public class QMBE_SupplierController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSuppliers")]
        public object GetFilteredSuppliers(Vanrise.Entities.DataRetrievalInput<SupplierQuery> input)
        {
            SupplierManager manager = new SupplierManager();
            return GetWebResponse(input, manager.GetFilteredSuppliers(input));
        }

        [HttpGet]
        [Route("GetSupplier")]
        public Supplier GetSupplier(int supplierId)
        {
            SupplierManager manager = new SupplierManager();
            return manager.GetSupplier(supplierId);
        }

        [HttpGet]
        [Route("GetSuppliersInfo")]
        public IEnumerable<SupplierInfo> GetSuppliersInfo()
        {
            SupplierManager manager = new SupplierManager();
            return manager.GetSuppliersInfo();
        }

        [HttpPost]
        [Route("AddSupplier")]
        public InsertOperationOutput<SupplierDetail> AddSupplier(Supplier whsSupplier)
        {
            SupplierManager manager = new SupplierManager();
            return manager.AddSupplier(whsSupplier);
        }
        [HttpPost]
        [Route("UpdateSupplier")]
        public UpdateOperationOutput<SupplierDetail> UpdateSupplier(Supplier whsSupplier)
        {
            SupplierManager manager = new SupplierManager();
            return manager.UpdateSupplier(whsSupplier);
        }

    }
}