using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Business.Connectors
{
    public class InventoryConnector : HttpConnector
    {
        protected override Guid GetConnectionId()
        {
            return new Guid("8D0E04EE-3917-4E3D-B0AB-EF56DA217745");
        }
    }
}
