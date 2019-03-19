using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace System.Web.Http
{
    //
    // Summary:
    //     Place on an action to expose it directly via a route.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class RouteAttribute : Attribute
    {
        public RouteAttribute(string template)
        {
        }

        //
        // Returns:
        //     Returns System.String.
        public string Name { get; set; }
        //
        // Returns:
        //     Returns System.Int32.
        public int Order { get; set; }
        //
        // Returns:
        //     Returns System.String.
        public string Template { get; }
    }
}
