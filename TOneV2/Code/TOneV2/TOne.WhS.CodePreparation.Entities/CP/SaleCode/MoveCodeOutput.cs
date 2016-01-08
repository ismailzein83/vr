﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class MoveCodeOutput
    {
        public string Message { get; set; }

        public List<CodeItem> NewCodes { get; set; }

        public CodePreparationOutputResult Result { get; set; }
    }
}
