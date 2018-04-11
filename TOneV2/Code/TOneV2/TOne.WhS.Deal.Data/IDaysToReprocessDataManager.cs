using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data
{
	public interface IDaysToReprocessDataManager : IDataManager
	{
		IEnumerable<DayToReprocess> GetAllDaysToReprocess();
		bool Insert(DateTime date, bool isSale, int carrierAccountId, out int insertedId);
		void DeleteDaysToReprocess();
		void DeleteDaysToReprocessByDate(DateTime date);
	}
}