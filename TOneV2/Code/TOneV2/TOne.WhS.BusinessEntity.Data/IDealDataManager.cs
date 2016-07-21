using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IDealDataManager : IDataManager
    {
        List<Deal> GetDeals();

        bool AreDealsUpdated(ref object updateHandle);

        bool Insert(Deal deal, out int insertedId);

        bool Update(Deal deal);
    }
}
