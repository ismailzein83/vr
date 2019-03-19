using System;
using System.Collections.Generic;
using System.Text;

namespace StandardWebLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GeneratedControllerAttribute : Attribute
    {
        public GeneratedControllerAttribute(string route)
        {
            Route = route;
        }

        public string Route { get; set; }
    }

    public class VRHttpPost : Attribute
    {

    }

    [GeneratedController("api/ModName/MyStandardAPI2/[action]")]
    public class MyStandardAPI2Controller //: Microsoft.AspNetCore.Mvc.Controller
    {
        public string GetItemFromArray(int itmIndex)
        {
            return new string[] { "Item 1", "Item 2", "Item 3" }[itmIndex];
        }
    }
}
