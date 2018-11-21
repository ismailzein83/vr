using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.MainExtension.Factory
{
	public class CarFactory : Demo.Module.Entities.Factory
	{
		public override Guid ConfigId { get { return new Guid("9DA9DEBD-5334-479C-8269-13CC3199E34C"); } }

		public string CarType { get; set; }
	}
}
