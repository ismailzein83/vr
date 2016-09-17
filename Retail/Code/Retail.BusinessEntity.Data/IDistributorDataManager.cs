using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Data
{
   public interface IDistributorDataManager : IDataManager
    {
        IEnumerable<Distributor> GetDistributors();

        bool Insert(Distributor distributor, out long insertedId);

        bool Update(Distributor distributor);

        bool AreDistributorsUpdated(ref object updateHandle);
    }
}
