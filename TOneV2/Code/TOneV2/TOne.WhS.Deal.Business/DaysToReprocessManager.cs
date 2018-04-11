using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Data;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
	public class DaysToReprocessManager
	{
		IDaysToReprocessDataManager _dataManager;

		public DaysToReprocessManager()
		{
			_dataManager = DealDataManagerFactory.GetDataManager<IDaysToReprocessDataManager>();
		}

		public IEnumerable<DayToReprocess> GetAllDaysToReprocess()
		{
			return _dataManager.GetAllDaysToReprocess();
		}

		public bool Insert(DateTime date, bool isSale, int carrierAccountId, out int insertedId)
		{
			return _dataManager.Insert(date, isSale, carrierAccountId, out insertedId);
		}

		public void DeleteDaysToReprocess()
		{
			_dataManager.DeleteDaysToReprocess();
		}

		public void DeleteDaysToReprocessByDate(DateTime date)
		{
			_dataManager.DeleteDaysToReprocessByDate(date);
		}
	}
}