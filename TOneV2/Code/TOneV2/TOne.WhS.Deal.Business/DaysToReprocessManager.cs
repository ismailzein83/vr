using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Data;

namespace TOne.WhS.Deal.Business
{
	public class DaysToReprocessManager
	{
		IDaysToReprocessDataManager _dataManager;

		public DaysToReprocessManager()
		{
			_dataManager = DealDataManagerFactory.GetDataManager<IDaysToReprocessDataManager>();
		}

		public bool Insert(DateTime date, bool isSale, int carrierAccountId, out int insertedId)
		{
			return _dataManager.Insert(date, isSale, carrierAccountId, out insertedId);
		}

		public void DeleteDaysToReprocess()
		{
			_dataManager.DeleteDaysToReprocess();
		}
	}
}
