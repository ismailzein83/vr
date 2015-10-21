using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class CodeCriteriaGroupSettings
    {
        public int ConfigId { get; set; }

        public abstract IEnumerable<CodeCriteria> GetCodeCriterias(CodeCriteriaGroupContext context);
    }

    public class SelectiveCodeCriteriaSettings : CodeCriteriaGroupSettings
    {
        public List<CodeCriteria> Codes { get; set; }

        public override IEnumerable<CodeCriteria> GetCodeCriterias(CodeCriteriaGroupContext context)
        {
            throw new NotImplementedException();
        }
    }
}
