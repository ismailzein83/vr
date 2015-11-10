using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
   public class CarrierProfileQuery
    {
       public List<int> CarrierProfileIds { get; set; }

       public string Name { get; set; }

       public List<int> CountriesIds { get; set; }

       public string Company { get; set; }

       public string RegistrationNumber { get; set; }

    }
}
