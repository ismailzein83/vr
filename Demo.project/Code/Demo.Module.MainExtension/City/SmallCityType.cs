using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.City
{
	public class SmallCityType : CityType
	{
		public override Guid ConfigId { get { return new Guid("F20740C9-F882-4B6B-91B5-ED620CEF209D"); } }

		public int NumberOfParcs { get; set; }

		public override string GetDescription()
		{
			return string.Format("Small city: {0} parcs ", this.NumberOfParcs);
		}
	}
}