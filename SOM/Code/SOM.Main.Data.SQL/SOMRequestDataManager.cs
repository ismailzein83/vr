using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace SOM.Main.Data.SQL
{
    public class SOMRequestDataManager : BaseSQLDataManager, ISOMRequestDataManager
    {
        #region ctor/Local Variables

        public SOMRequestDataManager()
            : base(GetConnectionStringName("SOMTransaction_DBConnStringKey", "SOMTransactionDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void AddRequest(Guid requestTypeId, string entityId, string serializedSettings, out long requestId)
        {
            Object idAsObj;
            ExecuteNonQuerySP("SOM.sp_SOMRequest_Insert", out idAsObj, requestTypeId, entityId, serializedSettings);
            requestId = (long)idAsObj;
        }

        public void UpdateRequestProcessInstanceId(long requestId, long processInstanceId)
        {
            ExecuteNonQuerySP("SOM.sp_SOMRequest_UpdateProcessInstanceID", requestId, processInstanceId);
        }

        #endregion
    }
}
