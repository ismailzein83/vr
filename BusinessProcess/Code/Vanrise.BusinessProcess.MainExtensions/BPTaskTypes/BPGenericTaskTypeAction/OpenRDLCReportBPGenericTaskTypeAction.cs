using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions
{
	public class OpenRDLCReportBPGenericTaskTypeAction: BPGenericTaskTypeActionSettings
	{
		public override string ActionTypeName { get { return "OpenRDLCReport"; } }
		public override Guid ConfigId { get { return new Guid("0E9FCADE-7E43-48F7-860D-3C563BBBB205"); } }
	}
}
