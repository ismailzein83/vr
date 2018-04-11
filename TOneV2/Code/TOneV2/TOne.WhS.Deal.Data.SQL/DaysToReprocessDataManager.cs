using System;
using System.Collections.Generic;
using System.Data;
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

		public IEnumerable<DayToReprocess> GetAllDaysToReprocess()
		{
			return GetItemsSP("TOneWhS_Deal.sp_DaysToReprocess_GetAll", DaysToReprocessMapper);
		}

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

		public void DeleteDaysToReprocessByDate(DateTime date)
		{
			ExecuteNonQuerySP("TOneWhS_Deal.sp_DaysToReprocess_DeleteByDate", date);
		}
		#endregion

		#region  Mappers

		private DayToReprocess DaysToReprocessMapper(IDataReader reader)
		{
			DayToReprocess daysToReprocess = new DayToReprocess
			{
				DayToReprocessId = (long)reader["ID"],
				Date = GetReaderValue<DateTime>(reader, "Date"),
				IsSale = (bool)reader["IsSale"],
				CarrierAccountId = (int)reader["CarrierAccountId"]
			};
			return daysToReprocess;
		}

		#endregion
	}
}