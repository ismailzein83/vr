using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Idb
{
    public interface IIdbBlockCustomerContext
    {
        string SwitchName { get; }
        List<string> CustomerMappings { get; }
        string ErrorMessage { set; }
    }

    public class IdbBlockCustomerContext : IIdbBlockCustomerContext
    {
        public string SwitchName { get; set; }
        public List<string> CustomerMappings { get; set; }
        public string ErrorMessage { get; set; }
    }
}