using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public enum ModuleTypeEnum { Voice = 0, SMS = 1 }
    public class ModuleTypeCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId { get { return new Guid("B5FC472D-56A6-4AC8-BC2A-3E183C98AC97"); } }

        public int ModuleType { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            TOneModuleManager toneModuleManager = new TOneModuleManager();
            if (ModuleType == (int)ModuleTypeEnum.Voice)
            {
                return toneModuleManager.IsVoiceModuleEnabled();
            }
            else
            {
                return toneModuleManager.IsSMSModuleEnabled();
            }
        }
    }
}
