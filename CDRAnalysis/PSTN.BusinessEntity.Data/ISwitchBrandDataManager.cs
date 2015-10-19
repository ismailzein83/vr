using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchBrandDataManager : IDataManager
    {
        List<SwitchBrand> GetBrands();

        Vanrise.Entities.BigResult<SwitchBrand> GetFilteredBrands(Vanrise.Entities.DataRetrievalInput<SwitchBrandQuery> input);

        SwitchBrand GetBrandById(int brandId);

        bool AddBrand(SwitchBrand brandObj, out int insertedId);

        bool UpdateBrand(SwitchBrand brandObj);

        bool DeleteBrand(int brandId);
    }
}
