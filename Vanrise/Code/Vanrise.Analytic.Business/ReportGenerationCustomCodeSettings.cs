using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Business
{
    public class ReportGenerationCustomCodeSettings : VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId { get { return new Guid("92B03E4E-37FB-48A1-ACEE-841135C30B5E"); } }

        public string CustomCode { get; set; }

        public string GetCustomCode()
        {
            return CustomCode;
        }
    }
  
  
}
