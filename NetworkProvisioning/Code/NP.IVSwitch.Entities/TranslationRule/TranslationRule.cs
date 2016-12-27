﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class TranslationRule
    {
        public int TranslationRuleId { get; set; }
        public String Name { get; set; }
        public String DNISPattern { get; set; }
        public String CLIPattern { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
