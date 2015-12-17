using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public enum MessageSeverity { Info = 0, Warning = 1, Error = 2};

    public interface IRuleTarget
    {
        MessageSeverity Severity { get; set; }

        string Message { get; }

        void SetExecluded();

        bool IsExecluded { get;  set; }

    }
}
