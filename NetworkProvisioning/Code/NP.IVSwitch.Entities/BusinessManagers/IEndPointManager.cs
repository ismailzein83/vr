using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public interface IEndPointManager : IBEManager
    {
        bool IsCacheExpired(ref DateTime? lastCheckTime);
    }
}
