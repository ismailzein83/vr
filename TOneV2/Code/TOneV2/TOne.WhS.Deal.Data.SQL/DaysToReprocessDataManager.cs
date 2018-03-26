using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Deal.Data.SQL
{
	public class DaysToReprocessDataManager : BaseSQLDataManager, IDaysToReprocessDataManager
	{
		#region Constructors

		public DaysToReprocessDataManager()
			: base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
		{

		}
		#endregion

		#region Public Methods

		public bool Insert(DateTime date, bool isSale, int carrierAccountId, out int insertedId)
		{
			object daysToReprocessId;

			int recordsEffected = ExecuteNonQuerySP("TOneWhS_Deal.sp_DaysToReprocess_Insert", out daysToReprocessId, date, isSale, carrierAccountId);
			bool insertedSuccesfully = (recordsEffected > 0);
			if (insertedSuccesfully)
				insertedId = (int)daysToReprocessId;
			else
				insertedId = 0;
			return insertedSuccesfully;
		}

		public void DeleteDaysToReprocess()
		{
			ExecuteNonQuerySP("TOneWhS_Deal.sp_DaysToReprocess_Delete");
		}
		#endregion

		#region  Mappers

		private DayToReprocess DaysToReprocessMapper(IDataReader reader)
		{
			DayToReprocess daysToReprocess = new DayToReprocess
			{
				DayToReprocessId = (int)reader["ID"],
				Date = GetReaderValue<DateTime>(reader, "Date"),
				IsSale = (bool)reader["IsSale"],
				CarrierAccountId = (int)reader["CarrierAccountId"]
			};
			return daysToReprocess;
		}

		#endregion
	}
}
