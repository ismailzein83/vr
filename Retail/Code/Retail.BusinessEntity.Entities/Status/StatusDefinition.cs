﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{    
    public class StatusDefinition
    {
        public Guid StatusDefinitionId { get; set; }

        public string Name { get; set; }
        
        public StatusDefinitionSettings Settings { get; set; }

        public EntityType EntityType { get; set; }
    }

    public class StatusDefinitionSettings
    {
       
    }
}