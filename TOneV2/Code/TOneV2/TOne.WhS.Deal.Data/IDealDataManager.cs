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
		IEnumerable<DealDefinition> GetDeals();

		bool AreDealsUpdated(ref object updateHandle);

		bool Insert(DealDefinition deal, out int insertedId);

		bool Update(DealDefinition deal);

		Byte[] GetMaxTimestamp();

		IEnumerable<DealDefinition> GetDealsModifiedAfterTimestamp(byte[] lastTimestamp);
	}
}
