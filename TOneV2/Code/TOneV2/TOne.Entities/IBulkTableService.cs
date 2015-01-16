﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace TOne.Entities
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBulkTableService" in both code and config file together.
    [ServiceContract]
    public interface IBulkTableService
    {
        [OperationContract]
        void WriteCodeMatchTable(DataTable dt);
    }
}
