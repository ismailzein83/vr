using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Idb
{
    public interface IIdbBlockCustomerContext
    {
        List<string> CustomerMappings { get; }
    }

    public class IdbBlockCustomerContext : IIdbBlockCustomerContext
    {
        public List<string> CustomerMappings { get; set; }
    }
}
