using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
	public class MultipleEndPointsToAdd
	{
		public List<EndPointToAdd> EndPointsToAdd { get;set;}
		public UserType EndPointType { get; set; }
	}
}
