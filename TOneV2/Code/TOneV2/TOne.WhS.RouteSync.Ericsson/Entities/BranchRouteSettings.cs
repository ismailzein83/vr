using System;
using System.Collections.Generic;
using System.Linq;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public abstract class BranchRouteSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract List<BaseBranchRoute> GetBaseBranchRoutes(IGetBaseBranchRoutesContext context);
        public abstract List<BRWithRouteCaseOptions> GetMergedBaseBranchRoutesWithRouteCaseOptions(IGetMergedBaseBranchRoutesWithRouteCaseOptionsContext context);
        public abstract List<BRWithRouteCaseOptions> GetBlockedBRWithRouteCaseOptions(IGetBlockedBRWithRouteCaseOptionsContext context);
    }
    public interface IGetMergedBaseBranchRoutesWithRouteCaseOptionsContext
    {
        List<BaseBRWithRouteCaseOptions> BaseBRWithRouteCaseOptions { get; }
    }
    public class GetMergedBaseBranchRoutesWithRouteCaseOptionsContext : IGetMergedBaseBranchRoutesWithRouteCaseOptionsContext
    {
        public List<BaseBRWithRouteCaseOptions> BaseBRWithRouteCaseOptions { get; set; }
    }

    public interface IGetBlockedBRWithRouteCaseOptionsContext
    {
    }
    public class GetBlockedBRWithRouteCaseOptionsContext : IGetBlockedBRWithRouteCaseOptionsContext
    {
    }

    public interface IGetBaseBranchRoutesContext
    {
    }
    public class GetBaseBranchRoutesContext : IGetBaseBranchRoutesContext
    {
    }

    public class RORangeBranchRouteSettings : BranchRouteSettings
    {
        public override Guid ConfigId { get { return new Guid("BA32DF95-FFE5-4C71-94FC-B02B841791F9"); } }

        public List<RORangeBranchRoute> RORangeBranchRoutes { get; set; }

        public override List<BaseBranchRoute> GetBaseBranchRoutes(IGetBaseBranchRoutesContext context)
        {
            List<BaseBranchRoute> baseBranchRoutes = new List<BaseBranchRoute>();
            baseBranchRoutes.AddRange(RORangeBranchRoutes);
            return baseBranchRoutes;
        }

        public override List<BRWithRouteCaseOptions> GetMergedBaseBranchRoutesWithRouteCaseOptions(IGetMergedBaseBranchRoutesWithRouteCaseOptionsContext context)
        {
            if (context.BaseBRWithRouteCaseOptions == null || context.BaseBRWithRouteCaseOptions.Count == 0)
                return null;

            var records = context.BaseBRWithRouteCaseOptions;
            var result = new List<BRWithRouteCaseOptions>();

            var orderdRecords = records.OrderBy(itm => (itm.BranchRoute as RORangeBranchRoute).From).ToList();

            var pre = new RORangeBRWithRouteCaseOptions()
            {
                BranchRoute = orderdRecords[0].BranchRoute as RORangeBranchRoute,
                RouteCaseOptions = orderdRecords[0].RouteCaseOptions
            };

            for (int i = 1; i < orderdRecords.Count; i++)
            {
                var cur = new RORangeBRWithRouteCaseOptions()
                {
                    BranchRoute = orderdRecords[i].BranchRoute as RORangeBranchRoute,
                    RouteCaseOptions = orderdRecords[i].RouteCaseOptions
                };

                if (pre.BranchRoute.To.HasValue && pre.BranchRoute.To == cur.BranchRoute.From - 1 && AreRouteCaseOptionsListsAreEqual(pre.RouteCaseOptions, cur.RouteCaseOptions))
                {
                    pre = new RORangeBRWithRouteCaseOptions()
                    {
                        BranchRoute = new RORangeBranchRoute { From = pre.BranchRoute.From, To = (cur.BranchRoute.To.HasValue) ? cur.BranchRoute.To : cur.BranchRoute.From, IncludeTrunkAsSwitch = cur.BranchRoute.IncludeTrunkAsSwitch, OverflowOnFirstOptionOnly = cur.BranchRoute.OverflowOnFirstOptionOnly },
                        RouteCaseOptions = cur.RouteCaseOptions
                    };
                }
                else if (!pre.BranchRoute.To.HasValue && pre.BranchRoute.From == cur.BranchRoute.From - 1 && AreRouteCaseOptionsListsAreEqual(pre.RouteCaseOptions, cur.RouteCaseOptions))
                {
                    pre = new RORangeBRWithRouteCaseOptions()
                    {
                        BranchRoute = new RORangeBranchRoute { From = pre.BranchRoute.From, To = (cur.BranchRoute.To.HasValue) ? cur.BranchRoute.To : cur.BranchRoute.From, IncludeTrunkAsSwitch = cur.BranchRoute.IncludeTrunkAsSwitch, OverflowOnFirstOptionOnly = cur.BranchRoute.OverflowOnFirstOptionOnly },
                        RouteCaseOptions = cur.RouteCaseOptions
                    };
                }
                else
                {
                    result.Add(new BRWithRouteCaseOptions() { BranchRoute = pre.BranchRoute.GetBranchRoute(), RouteCaseOptions = pre.RouteCaseOptions });
                    pre = new RORangeBRWithRouteCaseOptions()
                    {
                        BranchRoute = new RORangeBranchRoute { From = cur.BranchRoute.From, To = cur.BranchRoute.To, IncludeTrunkAsSwitch = cur.BranchRoute.IncludeTrunkAsSwitch, OverflowOnFirstOptionOnly = cur.BranchRoute.OverflowOnFirstOptionOnly },
                        RouteCaseOptions = cur.RouteCaseOptions
                    };
                }
            }

            result.Add(new BRWithRouteCaseOptions() { BranchRoute = pre.BranchRoute.GetBranchRoute(), RouteCaseOptions = pre.RouteCaseOptions });
            return result;
        }

        public override List<BRWithRouteCaseOptions> GetBlockedBRWithRouteCaseOptions(IGetBlockedBRWithRouteCaseOptionsContext context)
        {
            var blockedRouteCaseOption = new List<RouteCaseOption>() { Helper.GetBlockedRouteCaseOption() };
            var baseBRWithRouteCaseOptionsList = new List<BaseBRWithRouteCaseOptions>();

            foreach (var brachRoute in RORangeBranchRoutes)
            {
                baseBRWithRouteCaseOptionsList.Add(new BaseBRWithRouteCaseOptions() { BranchRoute = brachRoute, RouteCaseOptions = blockedRouteCaseOption });
            }

            return GetMergedBaseBranchRoutesWithRouteCaseOptions(new GetMergedBaseBranchRoutesWithRouteCaseOptionsContext() { BaseBRWithRouteCaseOptions = baseBRWithRouteCaseOptionsList });
        }

        private bool AreRouteCaseOptionsListsAreEqual(List<RouteCaseOption> list1, List<RouteCaseOption> list2)
        {
            if ((list1 == null || list1.Count == 0) && (list2 == null || list2.Count == 0))
                return true;

            if (list1 == null || list1.Count == 0)
                return false;

            if (list2 == null || list2.Count == 0)
                return false;

            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!AreRouteCaseOptionsEqual(list1[i], list2[i]))
                    return false;
            }

            return true;
        }

        private bool AreRouteCaseOptionsEqual(RouteCaseOption option1, RouteCaseOption option2)
        {
            if (option1 == null && option2 == null)
                return true;

            if (option1 == null)
                return false;

            if (option2 == null)
                return false;

            return (
                option1.GroupID == option2.GroupID &&
                option1.IsBackup == option2.IsBackup &&
                option1.IsSwitch == option2.IsSwitch &&
                option1.OutTrunk.Equals(option2.OutTrunk) &&
                option1.Percentage == option2.Percentage &&
                option1.SP == option2.SP &&
                string.Equals(option1.SupplierId, option2.SupplierId) &&
                option1.TrunkPercentage == option2.TrunkPercentage &&
                option1.Type == option2.Type &&
                option1.BNT == option2.BNT);
        }

    }

    public class FreeTextBranchRouteSettings : BranchRouteSettings
    {
        public override Guid ConfigId { get { return new Guid("BEC60C47-A37F-4EED-B628-1E17EFD60A41"); } }

        public List<FreeTextBranchRoute> FreeTextBranchRoutes { get; set; }

        public override List<BaseBranchRoute> GetBaseBranchRoutes(IGetBaseBranchRoutesContext context)
        {
            List<BaseBranchRoute> baseBranchRoutes = new List<BaseBranchRoute>();
            baseBranchRoutes.AddRange(FreeTextBranchRoutes);
            return baseBranchRoutes;
        }

        public override List<BRWithRouteCaseOptions> GetBlockedBRWithRouteCaseOptions(IGetBlockedBRWithRouteCaseOptionsContext context)
        {
            var blockedRouteCaseOption = new List<RouteCaseOption>() { Helper.GetBlockedRouteCaseOption() };
            var baseBRWithRouteCaseOptionsList = new List<BaseBRWithRouteCaseOptions>();

            foreach (var brachRoute in FreeTextBranchRoutes)
            {
                baseBRWithRouteCaseOptionsList.Add(new BaseBRWithRouteCaseOptions() { BranchRoute = brachRoute, RouteCaseOptions = blockedRouteCaseOption });
            }

            return GetMergedBaseBranchRoutesWithRouteCaseOptions(new GetMergedBaseBranchRoutesWithRouteCaseOptionsContext() { BaseBRWithRouteCaseOptions = baseBRWithRouteCaseOptionsList });
        }

        public override List<BRWithRouteCaseOptions> GetMergedBaseBranchRoutesWithRouteCaseOptions(IGetMergedBaseBranchRoutesWithRouteCaseOptionsContext context)
        {
            if (context.BaseBRWithRouteCaseOptions == null || context.BaseBRWithRouteCaseOptions.Count == 0)
                return null;

            var baseBRWithRouteCaseOptions = context.BaseBRWithRouteCaseOptions;
            var result = new List<BRWithRouteCaseOptions>();

            for (int i = 0; i < baseBRWithRouteCaseOptions.Count; i++)
                result.Add(new BRWithRouteCaseOptions() { BranchRoute = baseBRWithRouteCaseOptions[i].BranchRoute.GetBranchRoute(), RouteCaseOptions = baseBRWithRouteCaseOptions[i].RouteCaseOptions });

            return result;
        }
    }
}
