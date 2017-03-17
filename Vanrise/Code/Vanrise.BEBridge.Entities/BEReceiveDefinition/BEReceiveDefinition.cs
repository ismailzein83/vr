﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.BEBridge.Entities
{
    public class BEReceiveDefinition
    {
        public Guid BEReceiveDefinitionId { get; set; }

        public string Name { get; set; }

        public BEReceiveDefinitionSettings Settings { get; set; }

    }

    public class BEReceiveDefinitionSecurity
    {
        public RequiredPermissionSettings ViewPermission { get; set; }

        public RequiredPermissionSettings StartInstancePermission { get; set; }

    }
}
