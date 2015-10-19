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
        public IEnumerable<SwitchBrand> GetBrands()
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.GetAllSwitchBrands();
        }

        [HttpPost]
        public object GetFilteredBrands(Vanrise.Entities.DataRetrievalInput<SwitchBrandQuery> input)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return GetWebResponse(input, manager.GetFilteredSwitchBrands(input));
        }

        [HttpGet]
        public SwitchBrand GetBrandById(int brandId)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.GetSwitchBrandById(brandId);
        }

        [HttpPost]
        public InsertOperationOutput<SwitchBrand> AddBrand(SwitchBrand brandObj)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.AddBrand(brandObj);
        }

        [HttpPost]
        public UpdateOperationOutput<SwitchBrand> UpdateBrand(SwitchBrand brandObj)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.UpdateBrand(brandObj);
        }

        [HttpGet]
        public DeleteOperationOutput<object> DeleteBrand(int brandId)
        {
            SwitchBrandManager manager = new SwitchBrandManager();
            return manager.DeleteBrand(brandId);
        }
    }
}