using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class BrandController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<Brand> GetBrands()
        {
            BrandManager manager = new BrandManager();
            return manager.GetBrands();
        }

        [HttpPost]
        public object GetFilteredBrands(Vanrise.Entities.DataRetrievalInput<BrandQuery> input)
        {
            BrandManager manager = new BrandManager();
            return GetWebResponse(input, manager.GetFilteredBrands(input));
        }

        [HttpGet]
        public Brand GetBrandById(int brandId)
        {
            BrandManager manager = new BrandManager();
            return manager.GetBrandById(brandId);
        }

        [HttpPost]
        public InsertOperationOutput<Brand> AddBrand(Brand brandObj)
        {
            BrandManager manager = new BrandManager();
            return manager.AddBrand(brandObj);
        }

        [HttpPost]
        public UpdateOperationOutput<Brand> UpdateBrand(Brand brandObj)
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