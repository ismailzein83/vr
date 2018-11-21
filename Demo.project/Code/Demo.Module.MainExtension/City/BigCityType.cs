using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.City
{
	public class BigCityType : CityType
	{
		public override Guid ConfigId { get { return new Guid("C10FC8FF-C28A-4B80-BA12-65809163861C"); } }

		public int NumberOfCompanies { get; set; }

		public override string GetDescription()
		{
			return string.Format("Big city: {0} companies ", this.NumberOfCompanies);
		}
	}
}