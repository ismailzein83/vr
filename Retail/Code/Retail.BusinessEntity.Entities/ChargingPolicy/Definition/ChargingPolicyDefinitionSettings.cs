﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicyDefinitionSettings
    {
        public int ConfigId { get; set; }

        public virtual string ChargingPolicyEditor { get; set; }

        public List<ChargingPolicyRuleDefinition> RuleDefinitions { get; set; }

        public List<ChargingPolicyDefinitionPart> PartDefinitions { get; set; }
    }

    public class ChargingPolicyRuleDefinition
    {
        public string Title { get; set; }

        public int RuleDefinitionId { get; set; }
    }

    public class ChargingPolicyDefinitionPart
    {        
        public int PartTypeId { get; set; }

        public string PartTypeTitle { get; set; }

        public ChargingPolicyPartDefinitionSettings PartDefinitionSettings { get; set; }
    }
}
