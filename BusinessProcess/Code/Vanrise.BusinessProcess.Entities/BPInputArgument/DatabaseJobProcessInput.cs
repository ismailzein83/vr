using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class DatabaseJobProcessInput : BaseProcessInputArgument
    {
        public string ConnectionString { get; set; }
        public string  ConnectionStringName { get; set; }
        public string CustomCode { get; set; }

        public override string GetTitle()
        {
            throw new NotImplementedException();
        }
    }
}