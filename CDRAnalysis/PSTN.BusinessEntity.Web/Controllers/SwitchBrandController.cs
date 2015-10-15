using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class SwitchBrandController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<SwitchBrand> GetBrands()
        {
            BrandManager manager = new BrandManager();
            return manager.GetBrands();
        }

        [HttpPost]
        public object GetFilteredBrands(Vanrise.Entities.DataRetrievalInput<SwitchBrandQuery> input)
        {
            BrandManager manager = new BrandManager();
            return GetWebResponse(input, manager.GetFilteredBrands(input));
        }

        [HttpGet]
        public SwitchBrand GetBrandById(int brandId)
        {
            BrandManager manager = new BrandManager();
            return manager.GetBrandById(brandId);
        }

        [HttpPost]
        public InsertOperationOutput<SwitchBrand> AddBrand(SwitchBrand brandObj)
        {
            BrandManager manager = new BrandManager();
            return manager.AddBrand(brandObj);
        }

        [HttpPost]
        public UpdateOperationOutput<SwitchBrand> UpdateBrand(SwitchBrand brandObj)
        {
            BrandManager manager = new BrandManager();
            return manager.UpdateBrand(brandObj);
        }

        [HttpGet]
        public DeleteOperationOutput<object> DeleteBrand(int brandId)
        {
            BrandManager manager = new BrandManager();
            return manager.DeleteBrand(brandId);
        }
    }
}