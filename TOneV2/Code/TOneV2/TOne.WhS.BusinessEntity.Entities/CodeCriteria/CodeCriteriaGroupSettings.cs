using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class CodeCriteriaGroupSettings
    {
    }

    public class SelectiveCodeCriteriaSettings : CodeCriteriaGroupSettings
    {
        public List<CodeCriteria> Codes { get; set; }
    }
}
