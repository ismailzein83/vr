using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountPackageDataManager : IDataManager
    {
        IEnumerable<AccountPackage> GetAccountPackages();

        bool Insert(AccountPackage accountPackage, out int insertedId);

        bool Update(AccountPackageToEdit accountPackage);

        bool AreAccountPackagesUpdated(ref object updateHandle);
    }
}
