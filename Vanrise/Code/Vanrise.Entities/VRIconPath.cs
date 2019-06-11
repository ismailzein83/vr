using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Vanrise.Entities
{
    public class VRIconPath
    {
        public string Name { get; set; }
        public string IconPath { get; set; }
    }


    public enum VRIconVirtualPath {
        [Description("~/Images/figure-icons")]
        Figure = 1,
        [Description("~/Client/Images/menu-icons")]
        View = 2       
    }
}
