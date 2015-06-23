using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
   public  class PageSettings
    {
       public string PageName { get; set; }
       public AudienceWrapper AudianceIds { get; set; }
       public List<VisualElement> visualElements { get; set; }
    }
}
