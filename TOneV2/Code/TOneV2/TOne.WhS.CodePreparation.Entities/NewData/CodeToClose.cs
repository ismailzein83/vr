using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class CodeToClose : IRuleTarget
    {
        public string Code { get; set; }
        public TOne.WhS.BusinessEntity.Entities.CodeGroup CodeGroup { get; set; }
        public string ZoneName { get; set; }

        public DateTime CloseEffectiveDate { get; set; }

        public bool HasOverlapedCodesInOtherZone { get; set; }

        public bool IsExcluded { get; set; }


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

        public void SetExcluded()
        {
            this.IsExcluded = true;
        }

        public string TargetType
        {
            get { return "CodeToClose"; }
        }
    }
}
