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
    public class DealProgressDataManager : BaseSQLDataManager, IDealProgressDataManager
    {
        #region Constructors

        public DealProgressDataManager()
            : base(GetConnectionStringName("TOneWhS_Analytics_DBConnStringKey", "TOneAnalyticsDBConnString"))
        {

        }

        #endregion

        #region Public Methods


        #endregion

        #region  Mappers

        private DealProgress DealProgressMapper(IDataReader reader)
        {
            DealProgress dealProgress = new DealProgress
            {
                DealProgressID = (long)reader["ID"],
                DealID = (int)reader["ID"],
                ZoneGroupNb = (int)reader["ZoneGroupNb"],
                CurrentTierNb = (int)reader["CurrentTierNb"],
                CurrentRateTierNb = (int)reader["CurrentRateTierNb"],
                IsSale = (bool)reader["IsSale"],
                ReachedDurationInSeconds = GetReaderValue<decimal?>(reader, "ReachedDurationInSec"),
                TargetDurationInSeconds = (decimal)reader["TargetDurationInSec"],
                CreatedTime = (DateTime)reader["CreatedTime"]
            };
            return dealProgress;
        }

        #endregion
    }
}
