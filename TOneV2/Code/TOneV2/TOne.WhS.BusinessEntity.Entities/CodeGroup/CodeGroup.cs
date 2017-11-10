﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BaseCodeGroup : ICode
    {
        public int CodeGroupId { get; set; }

        public string Code { get; set; }

        public int CountryId { get; set; }
        public string Name { get; set; }
        public string SourceId { get; set; }
    }

    public class CodeGroup : BaseCodeGroup
    {
       
    }

    public class CodeGroupToEdit : BaseCodeGroup
    {

    }
}
