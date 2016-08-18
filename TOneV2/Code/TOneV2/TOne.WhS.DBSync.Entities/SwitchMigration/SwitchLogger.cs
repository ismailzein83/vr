using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class SwitchLogger
    {
        public bool Success { get; set; }
        public StringBuilder InfoMessage { get; set; }
        public StringBuilder WarningMessage { get; set; }
        public int InParsedMappingSuccededCount { get; set; }
        public int OutParsedMappingSuccededCount { get; set; }
        public int InParsedMappingFailedCount { get; set; }
        public int OutParsedMappingFailedCount { get; set; }
    }
}
