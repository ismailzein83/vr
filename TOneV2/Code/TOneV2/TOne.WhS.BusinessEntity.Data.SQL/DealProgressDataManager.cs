using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class DealProgressDataManager : BaseSQLDataManager, IDealProgressDataManager
    {
        #region Constructors

        public DealProgressDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<DealProgress> GetDealsProgress()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_DealProgress_GetAll]", DealProgressMapper) ;
        }

        public bool AreDealsProgressUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.DealProgress", ref updateHandle);
        }

        #endregion

        #region  Mappers

        private DealProgress DealProgressMapper(IDataReader reader)
        {
            DealProgress dealProgress = new DealProgress
            {
                DealProgressId = (int)reader["ID"],
                ProgressDate = GetReaderValue<DateTime>(reader, "Date"),
                IsSelling = GetReaderValue<bool>(reader, "IsSelling"),
                EstimatedDuration = GetReaderValue<decimal>(reader, "EstimatedDuration"),
                ReachedDuration = GetReaderValue<decimal>(reader, "ReachedDuration"),
                EstimatedAmount = GetReaderValue<decimal>(reader, "EstimatedAmount"),
                ReachedAmount = GetReaderValue<decimal>(reader, "ReachedAmount")

            };
            return dealProgress;
        }

        #endregion
    }
}
