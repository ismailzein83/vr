using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public interface IMediationProcessContext
    {
        void DeleteSessionId(string sessionId);
        string GetMultiLegSessionId(params string[] legIds);
    }
}
