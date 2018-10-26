using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Reprocess.Entities;

namespace TOne.WhS.Deal.MainExtensions.ReprocessFilterDefinition
{
    public class DealReprocessFilter : Vanrise.Reprocess.Entities.ReprocessFilter
    {
        public List<int> CustomerIds { get; set; }

        public List<int> SupplierIds { get; set; }
    }
}