using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class MasterLayoutSettingData : SettingData
    {
        public MasterLayoutMenuOption Menu { get; set; }

        public bool ExpandedMenu { get; set; }

        public bool IsBreadcrumbVisible { get; set; }

        public bool ShowApplicationTiles { get; set; }

        public bool ShowModuleTiles { get; set; }

    }
   
    public enum MasterLayoutMenuOption
    {
        
        FullMenu = 0,

        ModuleFilteredMenu = 1,

        NoMenu = 2
    }
}
