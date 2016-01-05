using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class Mapper : ITask
    {
        public void Execute()
        {
            Mappers.SQLMappers.FillData();
            Vanrise.Integration.Entities.MappingOutput output = Mappers.SQLMappers.StaggingCDRs_SQL(); 
        }
    }
}
