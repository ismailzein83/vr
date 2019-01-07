using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class ItemGroupingSection : InvoiceSubSectionSettings
    {
        public override Guid ConfigId { get { return new Guid("8A958396-18C2-4913-BABB-FF31683C6A17"); } }
        public Guid ItemGroupingId { get; set; }
        public ItemGroupingSectionSettings Settings { get; set; }
        public override Dictionary<Guid, string> GetSubsectionHeaders()
        {
            Dictionary<Guid, string> headers = new Dictionary<Guid, string>();
            if (Settings != null)
            {
                if (Settings.GridDimesions != null && Settings.GridDimesions.Count > 0)
                {
                    foreach(var dimension in Settings.GridDimesions)
                    {
                        headers.Add(dimension.DimensionId, dimension.Header);
                    }
                }
                if(Settings.GridMeasures!=null && Settings.GridMeasures.Count > 0)
                {
                    foreach(var measure in Settings.GridMeasures)
                    {
                        headers.Add(measure.MeasureId, measure.Header);
                    }
                }
            }
            return headers;
        }
    }
}
