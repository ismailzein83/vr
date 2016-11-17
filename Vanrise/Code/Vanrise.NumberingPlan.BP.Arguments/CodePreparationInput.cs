using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.BP.Arguments
{
    public class CodePreparationInput : BaseProcessInputArgument
    {
        public int SellingNumberPlanId { get; set; }
        public int? FileId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool HasHeader { get; set; }
        public bool IsFromExcel { get; set; }
        public override string GetTitle()
        {
            return null;
            //ISellingNumberPlanManager sellingNumberPlanManager = BEManagerFactory.GetManager<ISellingNumberPlanManager>();
            //string sellingNumberPlanName = sellingNumberPlanManager.GetSellingNumberPlanName(SellingNumberPlanId);
            //return String.Format("#BPDefinitionTitle# Process Started for Selling Number Plan: {0}", sellingNumberPlanName);
        }
    }
}