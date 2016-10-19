using System;
using Vanrise.Entities;
namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleActionType:ExtensionConfiguration
    {
      //  public Guid BPBusinessRuleActionTypeId { get; set; }\
        public const string EXTENSION_TYPE = "VR_BP_BPBusinessRuleActionType";
        public string Description { get; set; }
        public string Editor { get; set; }
    }
}
