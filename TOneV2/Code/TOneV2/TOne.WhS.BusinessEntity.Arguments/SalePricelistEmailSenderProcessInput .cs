using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.BP.Arguments
{
    public class SalePricelistEmailSenderProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public List<int> CustomerIds { get; set; }
        public Period Period { get; set; }

        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle#");
        }
    }
    public enum Period
    {
        Daily = 0,
        Monthly = 1,
        Yearly = 2
    }
}