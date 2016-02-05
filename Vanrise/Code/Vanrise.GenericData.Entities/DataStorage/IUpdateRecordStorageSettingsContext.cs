﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IUpdateRecordStorageContext
    {
        DataRecordStorage RecordStorage { get; }

        Object RecordStorageState { get; set; }
    }
}
