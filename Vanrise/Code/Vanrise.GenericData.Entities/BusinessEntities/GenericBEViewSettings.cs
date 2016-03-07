﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBEViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public int BusinessEntityDefinitionId { get; set; }

        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityManagement/{{\"businessEntityDefinitionId\":\"{0}\"}}", this.BusinessEntityDefinitionId);
        }
    }
}
