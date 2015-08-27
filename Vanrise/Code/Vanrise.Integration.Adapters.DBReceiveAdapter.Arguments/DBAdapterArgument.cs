﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments
{
    public class DBAdapterArgument : BaseAdapterArgument
    {
        public string ConnectionString { get; set; }

        public string Query { get; set; }
    }
}
