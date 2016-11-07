using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.BP.Arguments.Tasks
{
    public class NotificationTaskExecutionInforamtion : BPTaskExecutionInformation
    {
        public IEnumerable<int> CustomerIds { get; set; }
    }
}
