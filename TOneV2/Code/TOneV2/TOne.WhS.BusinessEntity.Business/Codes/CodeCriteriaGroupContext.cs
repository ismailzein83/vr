using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CodeCriteriaGroupContext : ICodeCriteriaGroupContext
    {
        public IEnumerable<CodeCriteria> GetGroupCodeCriterias(CodeCriteriaGroupSettings group)
        {
            return group.GetCodeCriterias(this);
        }
    }
}
