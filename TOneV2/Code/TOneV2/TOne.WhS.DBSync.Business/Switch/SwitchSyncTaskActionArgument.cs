using TOne.WhS.DBSync.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Business
{
    public class SwitchSyncTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public SourceSwitchReader SourceSwitchReader { get; set; }
    }
}
