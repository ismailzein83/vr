using System;

namespace Demo.Module.Entities
{
	public abstract class Factory
	{
		public abstract Guid ConfigId { get; }

		public int EmployeesNumber { get; set; }
	}
}
