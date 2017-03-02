using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRBusinessException : Exception
    {
        public VRBusinessException(string message)
            : base(message)
        {

        }

        public VRBusinessException(string message, Exception innerException) :base(message, innerException)
        {

        }
    }

    public class DataIntegrityValidationException : VRBusinessException
    {
        public DataIntegrityValidationException(string message)
            : base(message)
        {

        }
    }
}
