using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
   public class UserMeasuresValidatorInput
    {
       public List<int> UserIds { get; set; }
       public List<int> GroupIds { get; set; }
       public List<int> Widgets { get; set; }
    }
   public class UserMeasuresValidator
   {
       public int UserId { get; set; }
       public List<string> MeasuresDenied { get; set; }


   }
}
