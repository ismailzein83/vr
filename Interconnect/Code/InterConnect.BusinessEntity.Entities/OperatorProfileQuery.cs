using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterConnect.BusinessEntity.Entities
{
    public class OperatorProfileQuery
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public List<int> CountriesIds { get; set; }
    }
}
