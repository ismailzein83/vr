using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Business
{
    public class ReportGenerationFileSettings : Vanrise.Entities.VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("641D41B6-2EE6-43D1-8F09-2DB827F48A2F");  }
        }
        public Guid? TaskId { get; set; }
        public long? VRReportGenerationId { get; set; }
        public override bool DoesUserHaveViewAccess(Vanrise.Entities.IVRFileDoesUserHaveViewAccessContext context)
        {
            return true;
        }
    }
}
