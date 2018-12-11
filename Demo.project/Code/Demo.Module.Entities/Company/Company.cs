﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Company
    {
        public int CompanyId { get; set; }

        public string Name { get; set; }

        public CompanySettings Settings { get; set; }
    }

    public class CompanySettings
    {
        public string HeadquarterAddress { get; set; }
    }
}