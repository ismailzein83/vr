using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAlertRuleCriteria : VRAlertRuleCriteria
    {
        public override Guid ConfigId
        {
            get { return new Guid("711416A9-C0DA-4348-BEF5-9C877D84DF90"); }
        }

        public Guid DAProfCalcOutputItemDefinitionId { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }
    }
}
