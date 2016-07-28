﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectPropertyEvaluatorContext : IVRObjectPropertyEvaluatorContext
    {
        public dynamic Object
        {
            get;
            set;
        }

        public VRObjectType ObjectType
        {
            get;
            set;
        }
    }
}
