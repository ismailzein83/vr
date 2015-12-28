using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.BP.Arguments
{
    public class CodePreparationInput : BaseProcessInputArgument
    {
        public int SellingNumberPlanId { get; set; }
        public int? FileId { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public bool IsFromExcel { get; set; }
        public override string GetTitle()
        {
            return String.Format("CodePreparation Process Started for Package: {0}", SellingNumberPlanId);
        }
    }
}
