using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;

namespace TOne.WhS.Deal.Data.SQL
{
	public class DealDataManager : BaseSQLDataManager, IDealDataManager
	{
		#region Constructors

		public DealDataManager()
			: base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
		{

		}

		#endregion

		#region Public Methods

		public IEnumerable<DealDefinition> GetDeals()
		{

			return GetItemsSP("TOneWhS_Deal.sp_Deal_GetAll", DealMapper);
		}

		public bool AreDealsUpdated(ref object updateHandle)
		{
			return base.IsDataUpdated("TOneWhS_Deal.Deal", ref updateHandle);
		}

		public bool Insert(DealDefinition deal, out int insertedId)
		{
			object dealId;

			int recordsEffected = ExecuteNonQuerySP("TOneWhS_Deal.sp_Deal_Insert", out dealId, deal.Name, Vanrise.Common.Serializer.Serialize(deal.Settings));
			bool insertedSuccesfully = (recordsEffected > 0);
			if (insertedSuccesfully)
				insertedId = (int)dealId;
			else
				insertedId = 0;
			return insertedSuccesfully;
		}

		public bool Update(DealDefinition deal)
		{
			int recordsEffected = ExecuteNonQuerySP("TOneWhS_Deal.sp_Deal_Update", deal.DealId, deal.Name, Vanrise.Common.Serializer.Serialize(deal.Settings));
			return (recordsEffected > 0);
		}

		public Byte[] GetMaxTimestamp()
		{
			object maxTimestamp = ExecuteScalarSP("[TOneWhS_Deal].[sp_Deal_GetMaxTimeStamp]");
			if (maxTimestamp == null || maxTimestamp == DBNull.Value)
				return null;
			return (Byte[])maxTimestamp;
		}

		public DateTime? GetDealEvaluatorBeginDate(byte[] lastTimestamp, DateTime effectiveAfter)
		{
			IEnumerable<DealDefinition> deals = GetItemsSP("TOneWhS_Deal.sp_Deal_GetDealsModifiedAfterTimestamp", DealMapper, lastTimestamp);
			if (deals == null || !deals.Any())
				return null;
			var effectiveDeals = deals.Where(item => item.Settings.EndDate.VRGreaterThan(effectiveAfter));
			return effectiveDeals.MinBy(item => item.Settings.BeginDate).Settings.BeginDate;
			//return deals.Where(item => item.Settings.EndDate.VRGreaterThan(effectiveAfter)).MinBy(item => item.Settings.BeginDate).Settings.BeginDate;
		}

		#endregion

		#region  Mappers

		private DealDefinition DealMapper(IDataReader reader)
		{
			DealDefinition deal = new DealDefinition
			{
				DealId = (int)reader["ID"],
				Name = reader["Name"] as string,
				Settings = Vanrise.Common.Serializer.Deserialize<DealSettings>(reader["Settings"] as string)
			};
			return deal;
		}

		#endregion
	}
}
