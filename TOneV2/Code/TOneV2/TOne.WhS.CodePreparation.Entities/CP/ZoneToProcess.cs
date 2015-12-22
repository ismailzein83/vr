using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP.Processing
{
    public class ZoneToProcess
    {
        public string ZoneName { get; set; }

        public List<CodeToAdd> CodesToAdd { get; set; }

        public List<CodeToMove> CodesToMove { get; set; }

        public List<CodeToClose> CodesToClose { get; set; }
        public bool IsExcluded { get; set; }
    }
}
