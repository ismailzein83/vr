using System;

namespace Demo.Module.Entities
{
	public class Country
	{
		public int CountryId { get; set; }

		public string Name { get; set; }

		public CountrySettings Settings { get; set; }
	}

	public class CountrySettings
	{
		public string Capital { get; set; }

		public int Population { get; set; }
	}
}