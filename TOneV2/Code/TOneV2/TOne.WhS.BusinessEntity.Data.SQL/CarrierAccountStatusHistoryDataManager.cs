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
    public class CarrierAccountStatusHistoryDataManager : BaseSQLDataManager, ICarrierAccountStatusHistoryDataManager
    {
        #region ctor/Local Variables
        public CarrierAccountStatusHistoryDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }
        #endregion

        #region Public Methods

        public void Insert(int carrierAccountId, ActivationStatus status, ActivationStatus? previousStatus)
        {
            int? previousStatusId = null;
            if (previousStatus.HasValue)
                previousStatusId = (int)previousStatus;

            ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierAccountStatusHistory_Insert", carrierAccountId, (int)status, previousStatusId);
        }

        #endregion
    }
}