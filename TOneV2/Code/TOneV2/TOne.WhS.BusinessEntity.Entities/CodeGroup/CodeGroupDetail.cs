﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CodeGroupDetail
    {
        public CodeGroup Entity { get; set; }

        public string CountryName { get; set; }

        public bool AllowEdit { get; set; }
    }
}
