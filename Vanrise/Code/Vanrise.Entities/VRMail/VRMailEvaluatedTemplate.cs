﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRMailEvaluatedTemplate
    {
        public string To { get; set; }

        public string CC { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
