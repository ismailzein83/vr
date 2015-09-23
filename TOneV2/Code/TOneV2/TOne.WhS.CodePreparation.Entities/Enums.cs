using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public enum ImportType : short
    {
        New = 0,
        Delete = 1,
        Replace = -1
    }
}
