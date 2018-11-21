using System;

namespace Demo.Module.Entities
{
	public class District
	{
		public Guid DistrictId { get; set; }

		public string Name { get; set; }

		public int Population { get; set; }

		public DistrictSettings Settings { get; set; }
	}

	public abstract class DistrictSettings
	{
		public abstract Guid ConfigId { get; }
	}
}