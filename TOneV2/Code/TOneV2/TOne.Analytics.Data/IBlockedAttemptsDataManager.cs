using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IBlockedAttemptsDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<BlockedAttempts> GetBlockedAttempts(Vanrise.Entities.DataRetrievalInput<BlockedAttemptsInput> input);
    }
}
