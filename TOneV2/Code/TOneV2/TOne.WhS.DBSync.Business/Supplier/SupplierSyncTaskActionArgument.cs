using TOne.WhS.DBSync.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Business
{
    public class SupplierSyncTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public SourceSupplierReader SourceSupplierReader { get; set; }
    }
}
