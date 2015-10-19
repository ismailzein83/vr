using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchBrandDataManager : IDataManager
    {
        List<SwitchBrand> GetSwitchBrands();
        bool AddBrand(SwitchBrand brandObj, out int insertedId);
        bool UpdateBrand(SwitchBrand brandObj);
        bool DeleteBrand(int brandId);
        bool AreSwitchBrandsUpdated(ref object updateHandle);
    }
}
