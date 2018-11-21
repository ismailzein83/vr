using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.District
{
	public class IndustrialDistrict : DistrictSettings
	{
		public override Guid ConfigId { get { return new Guid("5B83F831-4E4D-4FCA-A6F6-3A023F2EC277"); } }

		public int FactoriesNumber { get; set; }

		public List<Entities.Factory> Factories { get; set; }
	}
}