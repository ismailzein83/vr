﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSource
    {
        public int DataSourceId { get; set; }

        public int AdapterTypeId { get; set; }

        public int TaskId { get; set; }

        public string AdapterName { get; set; }

        public DataSourceSettings Settings { get; set; }

    }

    public class DataSourceSettings
    {
        public BaseReceiveAdapter Adapter { get; set; }
    }
}
