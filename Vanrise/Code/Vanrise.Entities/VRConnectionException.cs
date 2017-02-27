using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRConnectionException : Exception
    {
        public VRConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
