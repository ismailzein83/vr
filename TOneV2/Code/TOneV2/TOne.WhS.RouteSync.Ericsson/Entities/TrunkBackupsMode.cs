using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public abstract class TrunkBackupsMode
    {
        public abstract Guid ConfigId { get; }

        public abstract void EvaluateTrunks(IEricssonTrunkBackupsModeContext context);

        protected List<RouteCaseOption> GetBackupsOption(IEricssonTrunkBackupsModeContext context)
        {
            var option = context.RouteOption;
            if (option.Backups == null || context.CurrentNumberOfMappings >= context.NumberOfMappings)
                return null;

            List<RouteCaseOption> routeCaseOptions = new List<RouteCaseOption>();
            var branchRoute = context.BranchRoute;

            foreach (var backupOption in option.Backups)
            {
                if (backupOption.IsBlocked)
                    continue;

                var backupSupplierMapping =  GetSupplierMapping(context.CarrierMappings, backupOption.SupplierId);
                if (backupSupplierMapping == null)
                    continue;

                var backupTrunkGroup =  GetTrunkGroup(context.RuleTree, backupOption.SupplierId, context.CodeGroupId, context.CustomerId, true);
                if (backupTrunkGroup == null)
                    continue;

                if (backupTrunkGroup.TrunkTrunkGroups != null && backupTrunkGroup.TrunkTrunkGroups.Count > 0)
                {
                    var outTrunksById = backupSupplierMapping.OutTrunks.ToDictionary(item => item.TrunkId, item => item);
                    foreach (var trunkGroupTrunk in backupTrunkGroup.TrunkTrunkGroups)
                    {
                        var trunk = outTrunksById.GetRecord(trunkGroupTrunk.TrunkId);

                        if (trunk.IsSwitch && branchRoute != null)
                        {
                            if (!branchRoute.IncludeTrunkAsSwitch)
                                continue;
                            if (branchRoute.OverflowOnFirstOptionOnly)
                                continue;
                        }

                        context.CurrentNumberOfMappings++;
                        routeCaseOptions.Add( GetRouteCaseOption(trunk, context.RouteCodeGroup, backupOption.SupplierId, null, backupSupplierMapping, backupTrunkGroup, trunkGroupTrunk, context.GroupId));

                        if (context.CurrentNumberOfMappings == context.NumberOfMappings)
                            break;
                    }
                }
            }

            return routeCaseOptions.Count > 0 ? routeCaseOptions : null;
        }

        protected void AddBackupsOption(IEricssonTrunkBackupsModeContext context)
        {
            var backupsOptions = GetBackupsOption(context);
            if (backupsOptions == null)
                return;

            context.RouteCaseOptions.AddRange(backupsOptions);
        }

        protected RouteCaseOption GetRouteCaseOption(OutTrunk trunk, string routeCodeGroup, string supplierId, int? percentage, SupplierMapping supplierMapping, TrunkGroup trunkGroup, TrunkTrunkGroup trunkGroupTrunk, int groupId)
        {
            RouteCaseOption routeCaseOption = new RouteCaseOption();
            int? routeCaseOptionPercentage = null;
            if (percentage.HasValue)
            {
                if (trunkGroupTrunk.Percentage.HasValue)
                    routeCaseOptionPercentage = Convert.ToInt32(percentage.Value * trunkGroupTrunk.Percentage.Value / 100.0);
            }

            routeCaseOption.Percentage = percentage;
            routeCaseOption.IsSwitch = trunk.IsSwitch;
            routeCaseOption.OutTrunk = trunk.TrunkName;
            routeCaseOption.Type = trunk.TrunkType;
            routeCaseOption.TrunkPercentage = routeCaseOptionPercentage;
            routeCaseOption.IsBackup = trunkGroup.IsBackup;
            routeCaseOption.GroupID = groupId;
            routeCaseOption.BNT = 1;
            routeCaseOption.SP = 1;
            routeCaseOption.SupplierId = supplierId;

            if (string.Compare(routeCodeGroup, trunk.NationalCountryCode) == 0)
            {
                routeCaseOption.BNT = 4;
                routeCaseOption.SP = Convert.ToInt16(trunk.NationalCountryCode.Length + 1);

            }

            return routeCaseOption;
        }

        protected List<RouteCaseOption> GetRouteCasesOption(IEricssonTrunkBackupsModeContext context, OutTrunk trunk, string supplierId, int? percentage, SupplierMapping supplierMapping, TrunkGroup trunkGroup, TrunkTrunkGroup trunkGroupTrunk)
        {
            List<RouteCaseOption> routeCasesOption = new List<RouteCaseOption>();
            int? routeCaseOptionPercentage = null;
            bool shouldAddBackupsTrunks = true;
            if (percentage.HasValue)
            {
                if (trunkGroupTrunk.Percentage.HasValue)
                    routeCaseOptionPercentage = Convert.ToInt32(percentage.Value * trunkGroupTrunk.Percentage.Value / 100.0);
            }
            else if (context.IsFirstSupplier && trunkGroupTrunk.Percentage.HasValue)
            {
                routeCaseOptionPercentage = Convert.ToInt32(trunkGroupTrunk.Percentage.Value);
                percentage = 100;
            }
            else
            {
                shouldAddBackupsTrunks = false;
            }

            RouteCaseOption routeCaseOption = new RouteCaseOption()
            {
                Percentage = percentage,
                IsSwitch = trunk.IsSwitch,
                OutTrunk = trunk.TrunkName,
                Type = trunk.TrunkType,
                TrunkPercentage = routeCaseOptionPercentage,
                IsBackup = trunkGroup.IsBackup,
                GroupID = context.GroupId,
                BNT = 1,
                SP = 1,
                SupplierId = supplierId
            };

            if (string.Compare(context.RouteCodeGroup, trunk.NationalCountryCode) == 0)
            {
                routeCaseOption.BNT = 4;
                routeCaseOption.SP = Convert.ToInt16(trunk.NationalCountryCode.Length + 1);
            }

            routeCasesOption.Add(routeCaseOption);
            context.CurrentNumberOfMappings++;

            if (context.CurrentNumberOfMappings == context.NumberOfMappings)
                return routeCasesOption;

            if (trunkGroupTrunk.Backups != null && shouldAddBackupsTrunks)
            {
                foreach (var backup in trunkGroupTrunk.Backups)
                {
                    if (backup.Trunks == null)
                        continue;

                    var backupSupplierMapping = GetSupplierMapping(context.CarrierMappings, backup.SupplierId.ToString());
                    if (backupSupplierMapping == null || backupSupplierMapping.OutTrunks == null || backupSupplierMapping.OutTrunks.Count == 0)
                        continue;

                    var outTrunksById = backupSupplierMapping.OutTrunks.ToDictionary(item => item.TrunkId, item => item);

                    foreach (var backupTrunk in backup.Trunks)
                    {
                        var outBackupTrunk = outTrunksById.GetRecord(backupTrunk.TrunkId);
                        if (outBackupTrunk.IsSwitch && context.BranchRoute != null)
                        {
                            if (!context.BranchRoute.IncludeTrunkAsSwitch)
                                continue;

                            if (context.BranchRoute.OverflowOnFirstOptionOnly)
                                continue;
                        }

                        context.CurrentNumberOfMappings++;
                        routeCasesOption.Add(new RouteCaseOption()
                        {
                            Percentage = null,
                            IsSwitch = outBackupTrunk.IsSwitch,
                            OutTrunk = outBackupTrunk.TrunkName,
                            Type = outBackupTrunk.TrunkType,
                            TrunkPercentage = null,
                            IsBackup = true,
                            GroupID = context.GroupId,
                            BNT = 1,
                            SP = 1,
                            SupplierId = backup.SupplierId.ToString()
                        });

                        if (context.CurrentNumberOfMappings == context.NumberOfMappings)
                            break;
                    }
                }
            }

            return routeCasesOption;
        }

        protected SupplierMapping GetSupplierMapping(Dictionary<string, CarrierMapping> carrierMappings, string supplierId)
        {
            if (carrierMappings == null || carrierMappings.Count == 0)
                return null;

            var supplierCarrierMapping = carrierMappings.GetRecord(supplierId);

            if (supplierCarrierMapping == null)
                return null;

            return supplierCarrierMapping.SupplierMapping;
        }

        protected TrunkGroup GetTrunkGroup(RuleTree ruleTree, string supplierId, int codeGroupId, string customerId, bool isBackup)
        {
            GenericRuleTarget target = new GenericRuleTarget();
            target.TargetFieldValues = new Dictionary<string, object>();
            target.TargetFieldValues.Add("Supplier", int.Parse(supplierId));
            target.TargetFieldValues.Add("CodeGroup", codeGroupId);
            target.TargetFieldValues.Add("Customer", int.Parse(customerId));
            target.TargetFieldValues.Add("IsBackUp", isBackup);

            TrunkGroupRuleAsGeneric matchingRule = Vanrise.GenericData.Business.GenericRuleManager<GenericRule>.GetMatchRule<TrunkGroupRuleAsGeneric>(ruleTree, null, target);
            if (matchingRule == null)
                return null;

            return matchingRule.TrunkGroup;
        }
    }

    public class BackupsPerTrunk : TrunkBackupsMode
    {
        public override Guid ConfigId { get { return new Guid("6E4D9F51-8EAD-499D-A118-71AA3A3EA077"); } }

        public override void EvaluateTrunks(IEricssonTrunkBackupsModeContext context)
        {
            var trunkGroup = context.TrunkGroup;
            var outTrunksById = context.OutTrunksById;
            var branchRoute = context.BranchRoute;
            var routeCaseOptions = context.RouteCaseOptions;
            var optionRouteCaseOptions = context.OptionRouteCaseOptions;
            var routeCodeGroup = context.RouteCodeGroup;
            bool isPercentageOption = context.IsPercentageOption;
            var routeCaseOptionWithSupplierList = context.RouteCaseOptionWithSupplierList;
            var option = context.RouteOption;
            bool isPercentageTrunkGroup = trunkGroup.TrunkTrunkGroups.Any(item => item.Percentage.HasValue && item.Percentage.Value > 0);

            if (trunkGroup.TrunkTrunkGroups != null && trunkGroup.TrunkTrunkGroups.Count > 0)
            {
                foreach (var trunkGroupTrunk in trunkGroup.TrunkTrunkGroups)
                {
                    var trunk = outTrunksById.GetRecord(trunkGroupTrunk.TrunkId);

                    if (trunk.IsSwitch && branchRoute != null)
                    {
                        if (!branchRoute.IncludeTrunkAsSwitch)
                            continue;

                        if (branchRoute.OverflowOnFirstOptionOnly && !context.IsFirstSupplier)
                            continue;
                    }

                    optionRouteCaseOptions.AddRange(GetRouteCasesOption(context, trunk, option.SupplierId, option.Percentage, null, trunkGroup, trunkGroupTrunk));

                    if (context.CurrentNumberOfMappings == context.NumberOfMappings)
                        break;

                    if (isPercentageTrunkGroup)
                    {
                        if (isPercentageOption)
                        {
                            var backupsOptions = GetBackupsOption(context);
                            if (backupsOptions != null)
                                optionRouteCaseOptions.AddRange(backupsOptions);
                        }
                        else if (!context.IsFirstSupplier)
                        {
                            continue;
                        }

                        context.GroupId++;
                        context.CurrentNumberOfMappings = 0;
                    }
                }

                if (optionRouteCaseOptions != null && optionRouteCaseOptions.Count > 0)
                {
                    if (option.Percentage.HasValue)
                        routeCaseOptionWithSupplierList.Add(new RouteCaseOptionWithSupplier() { SupplierId = option.SupplierId, Percentage = option.Percentage.Value, RouteCaseOptions = optionRouteCaseOptions });

                    routeCaseOptions.AddRange(optionRouteCaseOptions);
                    context.IsFirstSupplier = false;
                }
            }

            #region Option Backups

            if (!isPercentageTrunkGroup)
                AddBackupsOption(context);

            #endregion

            if (isPercentageOption)
            {
                if (context.CurrentNumberOfMappings > 0)
                    context.GroupId++;

                context.CurrentNumberOfMappings = 0;
            }
        }
    }

    public interface IEricssonTrunkBackupsModeContext
    {
        TrunkGroup TrunkGroup { get; }

        Dictionary<Guid, OutTrunk> OutTrunksById { get; }

        BaseBranchRoute BranchRoute { get; }

        bool IsFirstSupplier { get; set; }

        bool IsPercentageOption { get; }

        int NumberOfMappings { get; }

        int CurrentNumberOfMappings { get; set; }

        List<RouteCaseOptionWithSupplier> RouteCaseOptionWithSupplierList { get; set; }

        List<RouteCaseOption> RouteCaseOptions { get; set; }

        List<RouteCaseOption> OptionRouteCaseOptions { get; set; }

        string RouteCodeGroup { get; }

        RouteOption RouteOption { get; }

        int GroupId { get; set; }

        Dictionary<string, CarrierMapping> CarrierMappings { get; }

        RuleTree RuleTree { get; }

        int CodeGroupId { get; }

        string CustomerId { get; }
    }

    public class EricssonTrunkBackupsModeContext : IEricssonTrunkBackupsModeContext
    {
        public TrunkGroup TrunkGroup { get; set; }

        public Dictionary<Guid, OutTrunk> OutTrunksById { get; set; }

        public BaseBranchRoute BranchRoute { get; set; }

        public bool IsFirstSupplier { get; set; }

        public bool IsPercentageOption { get; set; }

        public int NumberOfMappings { get; set; }

        public List<RouteCaseOptionWithSupplier> RouteCaseOptionWithSupplierList { get; set; }

        public List<RouteCaseOption> RouteCaseOptions { get; set; }

        public List<RouteCaseOption> OptionRouteCaseOptions { get; set; }

        public int CurrentNumberOfMappings { get; set; }

        public string RouteCodeGroup { get; set; }

        public RouteOption RouteOption { get; set; }

        public int GroupId { get; set; }

        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public RuleTree RuleTree { get; set; }

        public int CodeGroupId { get; set; }

        public string CustomerId { get; set; }
    }
}