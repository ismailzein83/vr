﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericEditorField : GenericUIField
    {
        public bool IsRequired { get; set; }
        public bool IsDisabled { get; set; }
       // public bool IsDisabledOnEdit { get; set; }
    }
}
