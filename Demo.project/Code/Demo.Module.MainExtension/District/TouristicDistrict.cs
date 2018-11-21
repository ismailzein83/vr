using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.District
{
	public class TouristicDistrict : DistrictSettings
	{
		public override Guid ConfigId { get { return new Guid("0A389AEA-EB8A-4BD1-B7F2-363266B72A3F"); } }

		public int PalacesNumber { get; set; }
	}
}
