using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Data;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data
{
	public interface IDealZoneRateDataManager : IDataManager, IBulkApplyDataManager<DealZoneRate>
	{
		IEnumerable<DealZoneRate> GetDealZoneRatesByDate(bool isSale, DateTime fromDate, DateTime toDate);
		IEnumerable<DealZoneRate> GetDealZoneRatesByDealIds(bool isSale, IEnumerable<int> dealIds);
		bool AreDealZoneRateUpdated(ref object updateHandle);
		void InitializeDealZoneRateInsert(IEnumerable<int> dealIdsToKeep);
		void FinalizeDealZoneRateInsert();
		void ApplyNewDealZoneRatesToDB(object preparedRates);
	}
}