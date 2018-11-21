using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.MainExtension.Factory
{
	public class MilitaryFactory : Demo.Module.Entities.Factory
	{
		public override Guid ConfigId { get { return new Guid("7D1F8AAA-F832-4901-9DB3-7182677E4CB5"); } }

		public string MilitaryType { get; set; }
	}
}
