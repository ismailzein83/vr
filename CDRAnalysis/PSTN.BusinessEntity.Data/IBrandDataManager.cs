using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface IBrandDataManager : IDataManager
    {
        List<Brand> GetBrands();

        Vanrise.Entities.BigResult<Brand> GetFilteredBrands(Vanrise.Entities.DataRetrievalInput<BrandQuery> input);

        Brand GetBrandById(int brandId);

        bool AddBrand(Brand brandObj, out int insertedId);

        bool UpdateBrand(Brand brandObj);

        bool DeleteBrand(int brandId);
    }
}
