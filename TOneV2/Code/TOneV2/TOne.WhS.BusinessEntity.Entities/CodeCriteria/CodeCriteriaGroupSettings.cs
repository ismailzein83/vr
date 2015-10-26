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

        public abstract string GetDescription();
    }

    //public class SelectiveCodeCriteriaSettings : CodeCriteriaGroupSettings
    //{
    //    public List<CodeCriteria> Codes { get; set; }

    //    public override IEnumerable<CodeCriteria> GetCodeCriterias(CodeCriteriaGroupContext context)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetDescription()
    //    {
    //        if (this.Codes != null)
    //            return string.Join(",", this.Codes.Select(item => item.Code));

    //        return string.Empty;
    //    }
    //}
}
