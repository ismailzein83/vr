using System;
using System.Collections.Generic;
using System.Text;

namespace System.Web.Http
{
    //
    // Summary:
    //     Annotates a controller with a route prefix that applies to all actions within
    //     the controller.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RoutePrefixAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Web.Http.RoutePrefixAttribute class.
        //
        // Parameters:
        //   prefix:
        //     The route prefix for the controller.
        public RoutePrefixAttribute(string prefix)
        {
            this.Prefix = prefix;
        }

        //
        // Summary:
        //     Gets the route prefix.
        public string Prefix { get; }
    }
}
