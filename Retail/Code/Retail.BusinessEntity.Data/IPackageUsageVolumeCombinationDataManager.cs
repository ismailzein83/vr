using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Data;

namespace Retail.BusinessEntity.Data
{
    public interface IPackageUsageVolumeCombinationDataManager : IDataManager, IBulkApplyDataManager<PackageUsageVolumeCombination>
    {
        List<PackageUsageVolumeCombination> GetAllPackageUsageVolumeCombinations();

        Dictionary<int, PackageUsageVolumeCombination> GetPackageUsageVolumeCombinationAfterID(int id);

        void ApplyPackageUsageVolumeCombinationForDB(object preparedPackageUsageVolumeCombination);
    }
}