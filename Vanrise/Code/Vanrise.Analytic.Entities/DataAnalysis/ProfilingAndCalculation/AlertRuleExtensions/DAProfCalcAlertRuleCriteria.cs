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
        public Guid DAProfCalcOutputItemDefinitionId { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }



        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
