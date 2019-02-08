using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SMSBusinessEntity.BP.Arguments
{
    public class SMSSaleRateInput : BaseProcessInputArgument
    {
        public int CustomerID { get; set; }
        public override string GetTitle()
        {
            return string.Format("#BPDefinitionTitle# Applying SMS Sale Rate for Customer ID: {0} .", CustomerID);
        }
    }
}
