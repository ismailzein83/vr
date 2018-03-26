using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data
{
    public interface IDaysToReprocessDataManager : IDataManager
	{
		bool Insert(DateTime date, bool isSale, int carrierAccountId, out int insertedId);
		void DeleteDaysToReprocess();
	}
}
