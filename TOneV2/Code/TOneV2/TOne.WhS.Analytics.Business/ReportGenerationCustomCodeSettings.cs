using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business
{
    public class ReportGenerationCustomCodeSettings : VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("5E1CC97A-DAB5-4D4D-A092-F5540F7BA4DD"); }
        }
        public string CustomCode { get; set; }
        public string GetCustomCode()
        {
            return CustomCode;
        }
    }
}
