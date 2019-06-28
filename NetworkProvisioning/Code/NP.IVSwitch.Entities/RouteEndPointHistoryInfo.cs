using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
	public class RouteEndPointHistoryInfo
	{
		public SourceInfo Source { get; set; }
	}
	public enum SourceInfo { Manual=0,Automatically=1};
}
