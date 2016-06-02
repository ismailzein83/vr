using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.Package
{
    public class FixLineType : LineType
    {
        public FixConnectionType ConnectionType { get; set; }
    }
    public abstract class FixConnectionType
    {
        public int ConfigId { get; set; }
    }
}
