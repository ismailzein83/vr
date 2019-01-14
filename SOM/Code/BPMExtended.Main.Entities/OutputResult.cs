using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public enum ResultStatus { Error = 0 , Success = 1 , Warning = 2}

    public class OutputResult
    {
        public List<string> messages { get; set; }
        public ResultStatus status { get; set; }

    }




}
