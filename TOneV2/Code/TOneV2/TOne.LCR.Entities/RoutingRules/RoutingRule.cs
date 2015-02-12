using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public enum MultipleSelectionOption { All, AllExceptItems, OnlyItems }

    public class RoutingRule
    {
        public MultipleSelectionOption CarrierAccountSelectionOption { get; set; }

        public List<string> SelectedCarrierAccountIDs { get; set; }

        public BaseCodeSet CodeSet { get; set; }
    }
}