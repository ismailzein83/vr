using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary
{
    public partial class Operator
    {
        string fullname = "";
        public string FullName
        {
            get
            {
                if (fullname == "")
                {
                    fullname = this.Country + " - " + this.Name;
                }

                return fullname;
            }
        }
        
    }
}
