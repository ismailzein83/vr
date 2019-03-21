using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.BusinessProcess.Business.Extensions
{
	public class OpenBPInstanceViewerAction : GenericBEActionSettings
	{
		public override string ActionKind {
			get { return "OpenBPInstanceViewer"; }

		}
		public override Guid ConfigId {
			get { return new Guid("0AD405A3-9BCF-4796-9A1A-6CC2C3F9AAA3"); }
		}


	}
}

