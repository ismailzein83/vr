using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public class CodeToAdd : Vanrise.Entities.IDateEffectiveSettings, IRuleTarget
    {
        public string Code { get; set; }

        public string ZoneName { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public CodeGroup CodeGroup { get; set; }

        List<AddedCode> _addedCodes = new List<AddedCode>();
        public List<AddedCode> AddedCodes
        {
            get
            {
                return _addedCodes;
            }
        }

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
            get { return "CodeToAdd"; }
        }
    }
}
