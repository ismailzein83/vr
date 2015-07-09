﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public interface IDataMapper
    {
        Vanrise.Queueing.PersistentQueueItem MapData(IImportedData data);
    }
}
