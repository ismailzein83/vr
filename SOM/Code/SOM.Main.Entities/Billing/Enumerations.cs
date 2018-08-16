using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public enum CustomerType { Residential = 0, Enterprise = 1, Offical = 2 }

    public enum LineOfBusiness
    {
        Telephony = 0,

        LeasedLine = 1
    }

    public class LineOfBusinessAttribute : Attribute
    {
        public LineOfBusiness LOB { get; private set; }

        public LineOfBusinessAttribute(LineOfBusiness lob)
        {
            this.LOB = lob;
        }
    }
}
