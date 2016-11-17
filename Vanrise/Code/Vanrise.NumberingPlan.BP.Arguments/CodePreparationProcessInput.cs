using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.BP.Arguments
{
    public class CodePreparationProcessInput : BaseProcessInputArgument
    {
        public int SellingNumberPlanId { get; set; }
        public int FileId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool HasHeader { get; set; }
        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle# Process Started for Package: {0}", SellingNumberPlanId);
        }


    }
}
