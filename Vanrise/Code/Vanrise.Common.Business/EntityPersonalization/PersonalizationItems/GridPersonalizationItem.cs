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

        public List<GridColumnPersonalization> Columns { get; set; }
    }

    public class GridColumnPersonalization
    {
        public string FieldName { get; set; }
    }
}
