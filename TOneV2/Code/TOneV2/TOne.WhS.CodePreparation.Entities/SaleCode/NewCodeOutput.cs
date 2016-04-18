﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public class NewCodeOutput
    {
        public NewCodeOutput()
        {
            CodeItems = new List<CodeItem>();
        }
        public string Message { get; set; }
        public List<CodeItem> CodeItems { get; set; }
        public ValidationOutput Result { get; set; }
    }
}
