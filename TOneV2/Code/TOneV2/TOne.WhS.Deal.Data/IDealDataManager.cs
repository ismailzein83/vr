using System.Collections.Generic;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data
{
    public interface IDealDataManager : IDataManager
	{
		IEnumerable<DealDefinition> GetDeals();

		bool AreDealsUpdated(ref object updateHandle);
        bool Delete(int dealId);
		bool Insert(DealDefinition deal, out int insertedId);

		bool Update(DealDefinition deal);

        object GetMaxUpdateHandle();

        IEnumerable<DealDefinition> GetDealsModifiedAfterLastUpdateHandle(object lastDealDefinitionUpdateHandle);
	}
}