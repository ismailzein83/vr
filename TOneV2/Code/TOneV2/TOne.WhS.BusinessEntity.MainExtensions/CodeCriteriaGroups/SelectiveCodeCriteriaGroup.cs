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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("1deaf809-7eaa-4380-802f-aeb18c6a2368"); } }
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
