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
    public class DealDetailedProgressDataManager : BaseSQLDataManager, IDealDetailedProgressDataManager
    {
        #region Constructors

        public DealDetailedProgressDataManager()
            : base(GetConnectionStringName("TOneWhS_Analytics_DBConnStringKey", "TOneAnalyticsDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        #endregion

        #region  Mappers

        private DealDetailedProgress DealDetailedProgressMapper(IDataReader reader)
        {
            DealDetailedProgress dealDetailedProgress = new DealDetailedProgress
            {
                DealDetailedProgressID = (long)reader["ID"],
                DealID = (int)reader["ID"],
                ZoneGroupNb = (int)reader["ZoneGroupNb"],
                IsSale = (bool)reader["IsSale"],
                TierNb = (int)reader["TierNb"],
                RateTierNb = (int)reader["RateTierNb"],
                FromTime = (DateTime)reader["FromTime"],
                ToTime = (DateTime)reader["ToTime"],
                ReachedDurationInSeconds = GetReaderValue<decimal?>(reader, "ReachedDurationInSec"),
                CreatedTime = (DateTime)reader["CreatedTime"]
            };
            return dealDetailedProgress;
        }

        #endregion
    }
}
