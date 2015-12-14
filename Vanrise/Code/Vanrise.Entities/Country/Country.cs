using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class Country : EntitySynchronization.IItem
    {
       public int CountryId { get; set; }

       public string Name { get; set; }

       public string SourceId { get; set; }


       public long ItemId
       {
           get
           {
               return this.CountryId;
           }
           set
           {
               this.CountryId = (int)value;
           }
       }
    }
}
