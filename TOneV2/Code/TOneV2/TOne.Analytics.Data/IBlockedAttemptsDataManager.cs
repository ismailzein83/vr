using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Data
{
    public class IBlockedAttemptsDataManager: IDataManager
    {
        public Vanrise.Entities.BigResult<string> GetBlockedAttempts(Vanrise.Entities.DataRetrievalInput<string> input);
    }
}
