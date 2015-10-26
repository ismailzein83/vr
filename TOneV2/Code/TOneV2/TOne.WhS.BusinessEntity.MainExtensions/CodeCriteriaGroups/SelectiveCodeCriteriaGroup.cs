using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups
{
    public class SelectiveCodeCriteriaGroup : CodeCriteriaGroupSettings
    {
        public List<CodeCriteria> Codes { get; set; }

        public override IEnumerable<CodeCriteria> GetCodeCriterias(CodeCriteriaGroupContext context)
        {
            return this.Codes;
        }

        public override string GetDescription()
        {
            if (this.Codes != null)
                return string.Join(",", this.Codes.Select(item => item.Code));

            return string.Empty;
        }
    }
}
