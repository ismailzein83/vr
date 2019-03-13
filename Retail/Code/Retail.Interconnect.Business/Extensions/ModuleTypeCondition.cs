using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    public enum ModuleTypeEnum { Voice = 0, SMS = 1 }
    public class ModuleTypeCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId { get { return new Guid("B5FC472D-56A6-4AC8-BC2A-3E183C98AC97"); } }

        public int ModuleType { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            InterconnectModuleManager interconnectModuleManager = new InterconnectModuleManager();
            if (ModuleType == (int)ModuleTypeEnum.Voice)
            {
                return interconnectModuleManager.IsVoiceModuleEnabled();
            }
            else
            {
                return interconnectModuleManager.IsSMSModuleEnabled();
            }
        }
    }
}
