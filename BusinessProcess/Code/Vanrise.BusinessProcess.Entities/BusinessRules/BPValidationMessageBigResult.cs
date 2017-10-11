using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPValidationMessageBigResult : Vanrise.Entities.BigResult<BPValidationMessageDetail>
    {
        public bool HasWarningMessages { get; set; }
    }
}
