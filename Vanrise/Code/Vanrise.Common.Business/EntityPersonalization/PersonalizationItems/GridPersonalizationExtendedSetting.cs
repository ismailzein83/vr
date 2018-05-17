using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class GridPersonalizationExtendedSetting : EntityPersonalizationExtendedSetting
    {     
        public List<GridColumnVisibilityPersonalization> ColumnVisibilities { get; set; }
    }

    public class GridColumnVisibilityPersonalization
    {
        public string SysName { get; set; }

        public bool IsVisible { get; set; }
    }
}
