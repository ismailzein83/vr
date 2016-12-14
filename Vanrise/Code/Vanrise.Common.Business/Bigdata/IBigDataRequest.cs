using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    internal interface IBigDataRequest
    {
        int UserId { get; }

        string RetrieveData();
    }
}
