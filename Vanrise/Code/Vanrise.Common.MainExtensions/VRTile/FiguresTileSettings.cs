using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRTile
{
    public class FiguresTileSettings : VRTileExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("BE0BBEC6-F506-4805-AF5C-068843EF7481"); } }

        public override string RuntimeEditor { get { return "vr-common-figurestilesettings-runtime"; } }
        public Guid? ViewId { get; set; }
        public List<FiguresTileQuery> Queries { get; set; }
        public List<FiguresTileDisplayItem> ItemsToDisplay { get; set; }
    }
    //public class FiguresTileDisplayItem
    //{
    //    public Guid FiguresTileQueryId { get; set; }
    //    public string Name { get; set; }
    //    public string Title { get; set; }
    //    public string QueryName { get; set; }
    //}

}
