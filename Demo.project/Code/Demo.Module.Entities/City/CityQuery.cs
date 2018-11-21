using System.Collections.Generic;

namespace Demo.Module.Entities
{
	public class CityQuery
	{
		public string Name { get; set; }

		public List<int> CountryIds { get; set; }
	}
}