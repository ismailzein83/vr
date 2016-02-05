﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataStoreSettings
    {
        public int ConfigId { get; set; }

        public abstract void UpdateRecordStorage(IUpdateRecordStorageContext context);
    }
}
