using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskTypeFilter
    {
        public IEnumerable<IBPTaskTypeSettingsFilter> Filters { get; set; }
    }

    public interface IBPTaskTypeSettingsFilter
    {
        bool IsMatch(BPTaskType bPTaskType);
    }
}
