﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class NewCode : ICode
    {
        public long CodeId { get; set; }

        public string Code { get; set; }

        public IZone Zone { get; set; }

        public int CodeGroupId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public object Key
        {
            get { return Code; }
        }

        public string TargetType
        {
            get { return "Code"; }
        }
    }

}
