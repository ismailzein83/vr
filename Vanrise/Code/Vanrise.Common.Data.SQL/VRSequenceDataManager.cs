using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class VRSequenceDataManager : BaseSQLDataManager, IVRSequenceDataManager
    {
          #region ctor
        public VRSequenceDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion
        public long GetNextSequenceValue(string sequenceGroup, Guid sequenceDefinitionId, string sequenceKey, long initialValue, long? reserveNumber)
        {
            return (long)ExecuteScalarSP("common.sp_VRSequence_GetNext", sequenceGroup, sequenceDefinitionId, sequenceKey, initialValue, reserveNumber.HasValue? reserveNumber.Value: 1);
        }
    }
}
