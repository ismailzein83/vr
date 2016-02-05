using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace PSTN.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchBrand")]
    [JSONWithTypeAttribute]
    public class SwitchBrandController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetBrands")]
        public IEnumerable<SwitchBrand> GetBrands()
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.GetAllSwitchBrands();
        }

        [HttpPost]
        [Route("GetFilteredBrands")]
        public object GetFilteredBrands(Vanrise.Entities.DataRetrievalInput<SwitchBrandQuery> input)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return GetWebResponse(input, manager.GetFilteredSwitchBrands(input));
        }

        [HttpGet]
        [Route("GetBrandById")]
        public SwitchBrand GetBrandById(int brandId)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.GetSwitchBrandById(brandId);
        }

        [HttpPost]
        [Route("AddBrand")]
        public InsertOperationOutput<SwitchBrand> AddBrand(SwitchBrand brandObj)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.AddBrand(brandObj);
        }

        [HttpPost]
        [Route("UpdateBrand")]
        public UpdateOperationOutput<SwitchBrand> UpdateBrand(SwitchBrand brandObj)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.UpdateBrand(brandObj);
        }

        [HttpGet]
        [Route("DeleteBrand")]
        public DeleteOperationOutput<object> DeleteBrand(int brandId)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.DeleteBrand(brandId);
        }
    }
}