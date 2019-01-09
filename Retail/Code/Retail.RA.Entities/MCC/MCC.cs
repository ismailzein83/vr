using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class MCC
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int CountryId { get; set; }
    }
}
