using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ICodeCriteriaGroupContext : IContext
    {
        IEnumerable<CodeCriteria> GetGroupCodeCriterias(CodeCriteriaGroupSettings group);
    }
}
