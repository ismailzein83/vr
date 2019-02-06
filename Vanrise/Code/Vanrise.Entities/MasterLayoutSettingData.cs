using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class MasterLayoutSettingData 
    {
        MasterLayoutMenuOption _menuOption = MasterLayoutMenuOption.FullMenu;
        bool _expandedMenu = true;
        public MasterLayoutMenuOption MenuOption
        {
            get
            {
                return _menuOption;
            }
            set
            {
                _menuOption = value;
            }
        }

        public bool ExpandedMenu
        {
            get
            {
                return _expandedMenu;
            }
            set
            {
                _expandedMenu = value;
            }
        }

        public bool IsBreadcrumbVisible { get; set; }

        public bool ShowApplicationTiles { get; set; }

        public bool ShowModuleTiles { get; set; }

        public bool TilesMode { get; set; }

        public bool ModuleTilesMode { get; set; }

    }

    public enum MasterLayoutMenuOption
    {

        FullMenu = 0,

        ModuleFilteredMenu = 1,

        NoMenu = 2
    }
}
