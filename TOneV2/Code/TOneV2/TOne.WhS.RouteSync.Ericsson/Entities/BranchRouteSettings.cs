using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
	public abstract class BranchRouteSettings
	{
		public abstract Guid ConfigId { get; }
		public abstract List<BranchRoute> GenerateBranchRoutes(IGenerateBranchRoutesContext context);
	}

	public interface IGenerateBranchRoutesContext
	{
		List<RouteCaseOption> RouteCaseOptions { get; }
	}
	public class GenerateBranchRoutesContext : IGenerateBranchRoutesContext
	{
		public List<RouteCaseOption> RouteCaseOptions { get; set; }
	}

	public class RORangeBranchRouteSettings : BranchRouteSettings
	{
		public override Guid ConfigId { get { return new Guid("BA32DF95-FFE5-4C71-94FC-B02B841791F9"); } }

		public List<RORangeBranchRoute> RORangeBranchRoutes { get; set; }

		public override List<BranchRoute> GenerateBranchRoutes(IGenerateBranchRoutesContext context)
		{
			if (RORangeBranchRoutes == null || RORangeBranchRoutes.Count == 0)
				throw new NullReferenceException("Branch Routes are not defined");

			var routeCaseOptions = context.RouteCaseOptions;

			List<RORangeBranchRoute> result = new List<RORangeBranchRoute>();

			var mergedBranchRoutes = MergeBranchRoutes(RORangeBranchRoutes, true);

			var remergeAllBranchRoutes = false;
			if (mergedBranchRoutes != null && mergedBranchRoutes.Count > 1 && routeCaseOptions != null && routeCaseOptions.Count > 0)
				remergeAllBranchRoutes = !routeCaseOptions.Any(item => item.IsSwitch);

			if (remergeAllBranchRoutes)
				result = MergeBranchRoutes(mergedBranchRoutes, false);

			else result = mergedBranchRoutes;

			return (result != null || result.Count > 0) ? result.MapRecords((item) =>
			{
				return new BranchRoute()
				{
					Name = (item.To.HasValue) ? string.Format("RO-{0}&&-{1}", item.From, item.To) : string.Format("RO-{0}", item.From),
					IncludeTrunkAsSwitch = item.IncludeTrunkAsSwitch
				};
			}).ToList() : null;
		}

		public List<RORangeBranchRoute> MergeBranchRoutes(List<RORangeBranchRoute> branchRoutes, bool checkIncludeSwitch)
		{
			var orderedBRs = branchRoutes.OrderBy(item => item.From).ToList();
			var pre = orderedBRs[0];
			List<RORangeBranchRoute> result = new List<RORangeBranchRoute>();

			for (int i = 1; i < orderedBRs.Count; i++)
			{
				var cur = orderedBRs[i];
				if (pre.To.HasValue && pre.To == cur.From - 1 && (!checkIncludeSwitch || pre.IncludeTrunkAsSwitch == cur.IncludeTrunkAsSwitch))
				{
					pre = new RORangeBranchRoute { From = pre.From, To = cur.To, IncludeTrunkAsSwitch = !checkIncludeSwitch ? false : cur.IncludeTrunkAsSwitch };
				}
				else if (!pre.To.HasValue && pre.From == cur.From - 1 && (!checkIncludeSwitch || pre.IncludeTrunkAsSwitch == cur.IncludeTrunkAsSwitch))
				{
					pre = new RORangeBranchRoute { From = pre.From, To = cur.To, IncludeTrunkAsSwitch = !checkIncludeSwitch ? false : cur.IncludeTrunkAsSwitch };
				}
				else
				{
					result.Add(pre);
					pre = new RORangeBranchRoute() { From = cur.From, To = cur.To, IncludeTrunkAsSwitch = cur.IncludeTrunkAsSwitch };
				}
			}
			result.Add(pre);
			return result;
		}
	}

	public class FreeTextBranchRouteSettings : BranchRouteSettings
	{
		public override Guid ConfigId { get { return new Guid("BEC60C47-A37F-4EED-B628-1E17EFD60A41"); } }
		public List<FreeTextBranchRoute> FreeTextBranchRoutes { get; set; }
		public override List<BranchRoute> GenerateBranchRoutes(IGenerateBranchRoutesContext context)
		{
			if (FreeTextBranchRoutes == null || FreeTextBranchRoutes.Count == 0)
				throw new NullReferenceException("Branch Routes are not defined");

			return FreeTextBranchRoutes.MapRecords((item) =>
			{
				return new BranchRoute()
				{
					Name = item.Name,
					AlternativeName = item.AlternativeName,
					IncludeTrunkAsSwitch = item.IncludeTrunkAsSwitch
				};
			}).ToList();
		}
	}
}
