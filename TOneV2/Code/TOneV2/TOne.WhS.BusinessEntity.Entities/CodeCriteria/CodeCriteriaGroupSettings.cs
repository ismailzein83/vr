using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class CodeCriteriaGroupSettings
    {
        public virtual Guid ConfigId { get; set; }

        public abstract IEnumerable<CodeCriteria> GetCodeCriterias(ICodeCriteriaGroupContext context);

        public abstract string GetDescription(ICodeCriteriaGroupContext context);
    }
}
