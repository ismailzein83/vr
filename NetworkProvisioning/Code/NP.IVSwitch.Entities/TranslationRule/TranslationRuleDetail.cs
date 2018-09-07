﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class TranslationRuleDetail
    {
        public TranslationRule Entity { get; set; }
        public String Name { get; set; }
        public EngineType EngineType { get; set; }
        public PrefixSign? PrefixSign { get; set; }
        public String DNISPattern { get; set; }
        public CLIType CLIType { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
