using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Security.Entities
{
    public class ViewType:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Security_ViewTypeConfig";

     //   public Guid ViewTypeId { get; set; }
       // public string Name { get; set; }
      //  public string Title { get; set; }
        public string Editor { get; set; }
        public bool EnableAdd { get; set; }
    }
}
