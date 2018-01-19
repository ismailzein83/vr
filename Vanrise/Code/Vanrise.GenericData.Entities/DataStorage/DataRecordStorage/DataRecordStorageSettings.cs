﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordStorageSettings
    {
        public string DateTimeField { get; set; }
        public bool EnableUseCaching { get; set; }
        public RequiredPermissionSettings RequiredPermission { get; set; }

    }
}
