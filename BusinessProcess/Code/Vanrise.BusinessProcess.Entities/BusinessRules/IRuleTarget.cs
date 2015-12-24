using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IRuleTarget
    {
        object Key { get; }

        string Message { get; }

        void SetExcluded();

    }
}
