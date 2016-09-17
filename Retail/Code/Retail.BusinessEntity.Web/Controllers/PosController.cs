using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Entities.POS;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Pos")]
    public class PosController : BaseAPIController
    {
        [HttpGet]
        [Route("GetPointOfSalesInfo")]
        public IEnumerable<POSInfo> GetPointOfSalesInfo()
        {
            POSManager manager = new POSManager();
            return manager.GetPointOfSalesInfo(null);
        }

        [HttpPost]
        [Route("AddPointOfSale")]
        public Vanrise.Entities.InsertOperationOutput<PosDetail> AddPointOfSale(PointOfSale pos)
        {
            POSManager manager = new POSManager();
            return manager.AddPointOfSale(pos);
        }

        [HttpPost]
        [Route("UpdatePointOfSale")]
        public Vanrise.Entities.UpdateOperationOutput<PosDetail> UpdatePointOfSale(PointOfSale pos)
        {
            POSManager manager = new POSManager();
            return manager.UpdatePointOfSale(pos);
        }

        [HttpPost]
        [Route("GetFilteredPointOfSales")]
        public object GetFilteredPointOfSales(Vanrise.Entities.DataRetrievalInput<POSQuery> input)
        {
            POSManager distributorManager = new POSManager();
            return GetWebResponse(input, distributorManager.GetFilteredPointOfSales(input));
        }
    }
}