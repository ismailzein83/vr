﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IDataRecordStorageManager : IBusinessManager
    {
        bool DoesUserHaveAccess(int userId,List<int> dataRecordStorageIds );
    }
}
