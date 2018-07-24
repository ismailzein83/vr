using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{    
    public enum VRHttpMethod
    {
        Get = 0,
        Post = 1,
        Put = 2,
        Delete = 3
    }

    public enum VRHttpMessageFormat
    {
        [VRHttpMessageFormat("application/json")]
        ApplicationJSON = 0,
        [VRHttpMessageFormat("application/xml")]
        ApplicationXML = 1,
        [VRHttpMessageFormat("text/xml")]
        TextXML = 2
    }

    public class VRHttpMessageFormatAttribute : Attribute
    {
        public VRHttpMessageFormatAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }
    public class VRHttpConnectionHeader
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
