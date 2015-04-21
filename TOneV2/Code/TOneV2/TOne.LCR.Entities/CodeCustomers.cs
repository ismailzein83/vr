using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    /// <summary>
    ///Key is Code, Value is HashSet of Customer IDs
    /// </summary>
    public class CodeCustomers : Dictionary<string, HashSet<string>>
    {

    }
}
