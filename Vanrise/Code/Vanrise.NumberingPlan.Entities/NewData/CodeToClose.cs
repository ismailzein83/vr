using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public class CodeToClose : IRuleTarget
    {
        public string Code { get; set; }
        public CodeGroup CodeGroup { get; set; }
        public string ZoneName { get; set; }

        public DateTime CloseEffectiveDate { get; set; }

        public bool HasOverlapedCodesInOtherZone { get; set; }


        List<ExistingCode> _changedExistingCodes = new List<ExistingCode>();

        public List<ExistingCode> ChangedExistingCodes
        {
            get
            {
                return _changedExistingCodes;
            }
        }

        public object Key
        {
            get { return this.Code; }
        }

        public string TargetType
        {
            get { return "CodeToClose"; }
        }
    }
}
