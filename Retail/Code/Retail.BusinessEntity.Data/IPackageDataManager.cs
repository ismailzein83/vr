using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IPackageDataManager:IDataManager
    {
        List<Package> GetPackages();

        bool Insert(Package package, out int insertedId);

        bool Update(Package package);

        bool ArePackagesUpdated(ref object updateHandle);
    }
}
