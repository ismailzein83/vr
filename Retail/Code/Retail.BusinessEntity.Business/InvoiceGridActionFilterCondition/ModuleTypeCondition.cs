using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.BusinessEntity.Business
{
    public enum ModuleTypeEnum { Voice = 0, SMS = 1}
    public class ModuleTypeCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId { get { return new Guid("B5FC472D-56A6-4AC8-BC2A-3E183C98AC97"); } }

        public int ModuleType { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            RetailModuleManager retailModuleManager = new RetailModuleManager();
            Guid voiceAnalyticTableId = new Guid("6cd535c0-ac49-46bb-aecf-0eae33823b20");
            Guid smsAnalyticTableId = new Guid("c1bd3f2f-6213-44d1-9d58-99f81e169930");
            if (ModuleType == (int)ModuleTypeEnum.Voice)
            {
                return retailModuleManager.IsVoiceModuleEnabled(voiceAnalyticTableId);
            }
            else
            {
                return retailModuleManager.IsSMSModuleEnabled(smsAnalyticTableId);
            }
        }
    }
}
