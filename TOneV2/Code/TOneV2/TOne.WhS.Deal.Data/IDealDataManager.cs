using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data
{
    public interface IDealDataManager : IDataManager
    {
        List<TOne.WhS.Deal.Entities.Deal> GetDeals();

        bool AreDealsUpdated(ref object updateHandle);

        bool Insert(TOne.WhS.Deal.Entities.Deal deal, out int insertedId);

        bool Update(TOne.WhS.Deal.Entities.Deal deal);
    }
}
