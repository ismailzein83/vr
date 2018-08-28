namespace TOne.WhS.RouteSync.Ericsson.Entities
{
	public class BranchRoute
	{
		public string Name { get; set; }
		public string AlternativeName { get; set; }
		public bool IncludeTrunkAsSwitch { get; set; }
	}

	public class RORangeBranchRoute
	{
		public int From { get; set; }
		public int? To { get; set; }
		public bool IncludeTrunkAsSwitch { get; set; }
	}

	public class FreeTextBranchRoute
	{
		public string Name { get; set; }
		public string AlternativeName { get; set; }
		public bool IncludeTrunkAsSwitch { get; set; }
	}
}
