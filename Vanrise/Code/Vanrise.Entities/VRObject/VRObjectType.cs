﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRObjectType
    {
        public int ConfigId { get; set; }

        public abstract string PropertyEvaluatorExtensionType { get; }
    }
}
