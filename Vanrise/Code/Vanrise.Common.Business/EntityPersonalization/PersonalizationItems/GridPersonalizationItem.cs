using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class GridPersonalizationItem : EntityPersonalizationItemSetting
    {
        public override string Title
        {
            get { return "Grid"; }
        }

        public List<GridColumnVisibilityPersonalization> ColumnVisibilities { get; set; }
    }

    public class GridColumnVisibilityPersonalization
    {
        public string FieldName { get; set; }

        public bool IsVisible { get; set; }
    }
}
