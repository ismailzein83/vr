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
        public const int ExtensionConfigId = 5;
        public List<CodeCriteria> Codes { get; set; }

        public override IEnumerable<CodeCriteria> GetCodeCriterias(ICodeCriteriaGroupContext context)
        {
            return this.Codes;
        }

        public override string GetDescription(ICodeCriteriaGroupContext context)
        {
            var validCodes = context != null ? context.GetGroupCodeCriterias(this) : this.Codes;
            if (validCodes != null)
                return string.Join(", ", validCodes.Select(item => item.Code));
            else
                return null;
        }
    }
}
