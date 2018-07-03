using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class VRSequenceManager
    {
        public long GetNextSequenceValue(string sequenceGroup, Guid sequenceDefinitionId, string sequenceKey, long initialValue, long? reserveNumber = null)
        {
            IVRSequenceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRSequenceDataManager>();
            return dataManager.GetNextSequenceValue(sequenceGroup, sequenceDefinitionId, sequenceKey, initialValue, reserveNumber);
        }
    }
}
