using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public interface IAnalyticItemConfigManager : IBEManager
    {
        bool DoesUserHaveAccess(int userId, int analyticTableId, List<string> measureNames);
    }
}
