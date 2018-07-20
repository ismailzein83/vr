using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public interface IVRHttpConnection
    {

        VRHttpConnectionInfo GetConnectionInfo();
    }

    public class VRHttpConnectionInfo
    {
        public string URL { get; set; }

        public List<VRHttpConnectionHeader> Headers { get; set; }
    }

    public class VRHttpConnectionHeader
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
