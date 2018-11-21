using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
	public class City
	{
		public int CityId { get; set; }

		public string Name { get; set; }

		public int CountryId { get; set; }

		public CitySettings Settings { get; set; }
	}

	public  class CitySettings
	{
		public int Population { get; set; }

		public CityType CityType { get; set; }

		public List<District> Districts { get; set; }
	}

	public abstract class CityType
	{
		public abstract Guid ConfigId { get; }

		public abstract string GetDescription();
	}
}