using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class SupplierCodes
    {
        /// <summary>
        /// Key is Code number, value is Code details
        /// </summary>
        public Dictionary<string, Code> Codes { get; set; }
    }
}
