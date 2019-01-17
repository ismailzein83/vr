using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class UtilityDataManager : IUtilityDataManager
	{
		#region Local Variables
		#endregion

		#region Constructors
		#endregion

		#region Public Methods
		public bool CheckIfDefaultOrInvalid(DateTime? dateTime)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.CheckIfDefaultOrInvalidDBTime(dateTime);
		}

		public DateTimeRange GetDateTimeRange()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.GetDBDateTimeRange();
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("vrCommon", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}

		#endregion

		#region Mappers
		#endregion

	}
}
