using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class VolumePackageSettings : PackageExtendedSettings, Retail.Voice.Entities.IPackageSettingVoiceUsageCharger
    {
        public override Guid ConfigId
        {
            get { return new Guid("E8EC8C13-DC47-46F1-95FE-082F4760A0A0"); }
        }

        public List<VoiceVolumePackageItem> VoiceItems { get; set; }

        public List<SMSVolumePackageItem> SMSItems { get; set; }

        public List<DataVolumePackageItem> DataItems { get; set; }

        static BalanceManager s_balanceManager = new BalanceManager();

        public bool TryGetVoiceUsageCharger(Guid serviceTypeId, out Voice.Entities.IPackageVoiceUsageCharger voiceUsageCharging)
        {
            if (this.VoiceItems != null)
            {
                List<VoiceVolumePackageItem> matchServiceVoiceItems = this.VoiceItems.FindAllRecords(itm => itm.ServiceTypeId == serviceTypeId).ToList();
                if (matchServiceVoiceItems != null && matchServiceVoiceItems.Count > 0)
                {
                    voiceUsageCharging = new VoiceUsageCharger(matchServiceVoiceItems);
                    return true;
                }
            }
            voiceUsageCharging = null;
            return false;
        }

        public override void OnPackageAssignmentStarted(IPackageSettingAssignementStartedContext context)
        {
            //Create Balances
            if (this.VoiceItems != null)
            {
                foreach (var voicePackageItem in this.VoiceItems)
                {
                    s_balanceManager.CreateVolumeBalance(context.AccountId, GetVolumeItemBalanceKey(voicePackageItem), voicePackageItem.VolumeInMin);
                }
            }
            if (this.SMSItems != null)
            {
                foreach (var smsPackageItem in this.SMSItems)
                {
                    s_balanceManager.CreateVolumeBalance(context.AccountId, GetVolumeItemBalanceKey(smsPackageItem), smsPackageItem.SMSCount);
                }
            }
            if (this.DataItems != null)
            {
                foreach (var dataPackageItem in this.DataItems)
                {
                    s_balanceManager.CreateVolumeBalance(context.AccountId, GetVolumeItemBalanceKey(dataPackageItem), dataPackageItem.VolumeInMB);
                }
            }
        }

        static string GetVolumeItemBalanceKey(VolumePackageItem volumePackageItem)
        {
            return volumePackageItem.ItemId.ToString();
        }

        #region Classes

        private class VoiceUsageCharger : Retail.Voice.Entities.IPackageVoiceUsageCharger
        {
            List<VoiceVolumePackageItem> _voiceVolumeItems;
            public VoiceUsageCharger(List<VoiceVolumePackageItem> voiceVolumeItems)
            {
                _voiceVolumeItems = voiceVolumeItems;
            }


            public void TryChargeVoiceEvent(Voice.Entities.IVoiceUsageChargerContext context)
            {
                List<DurationToDeduct> volumesToDeduct = new List<DurationToDeduct>();
                Decimal remainingDurationToCharge = context.Duration;
                foreach (var voiceVolumeItem in _voiceVolumeItems)
                {
                    if (voiceVolumeItem.EventCondition != null)
                    {
                        var conditionContext = new AccountBillingEventConditionContext(context.AccountId)
                        {
                            BillingEvent = context.MappedCDR
                        };
                        if (!voiceVolumeItem.EventCondition.IsMatch(conditionContext))
                            continue;
                    }
                    string itemKey = GetVolumeItemBalanceKey(voiceVolumeItem);
                    var remaingBalance = s_balanceManager.GetAccountRemainingBalance(context.AccountId, itemKey);
                    if (remaingBalance > 0)
                    {
                        Decimal chargeableDuration = Math.Min(remainingDurationToCharge, remaingBalance);
                        remainingDurationToCharge -= chargeableDuration;
                        volumesToDeduct.Add(new DurationToDeduct
                        {
                            VolumeItemKey = itemKey,
                            Duration = chargeableDuration
                        });
                        if (remainingDurationToCharge == 0)
                            break;
                    }
                }
                context.ChargeInfo = volumesToDeduct;
            }

            public void DeductFromBalances(Voice.Entities.IVoiceUsageChargerDeductFromBalanceContext context)
            {
                List<DurationToDeduct> volumesToDeduct = context.ChargeInfo as List<DurationToDeduct>;
                if (volumesToDeduct != null)
                {
                    foreach (var volumeToDeduct in volumesToDeduct)
                    {
                        s_balanceManager.DeductFromAccountBalance(context.AccountId, volumeToDeduct.VolumeItemKey, volumeToDeduct.Duration);
                    }
                }
            }

            private class DurationToDeduct
            {
                public string VolumeItemKey { get; set; }

                public Decimal Duration { get; set; }
            }
        }

        #endregion
    }

    public class VolumePackageItem
    {
        public Guid ItemId { get; set; }

        public string Title { get; set; }

        public Guid ServiceTypeId { get; set; }

        public AccountBillingEventCondition EventCondition { get; set; }
    }

    public class VoiceVolumePackageItem : VolumePackageItem
    {
        public Decimal VolumeInMin { get; set; }
    }

    public class SMSVolumePackageItem : VolumePackageItem
    {
        public int SMSCount { get; set; }
    }

    public class DataVolumePackageItem : VolumePackageItem
    {
        public int VolumeInMB { get; set; }
    }

    public abstract class AccountBillingEventCondition
    {
        public abstract bool IsMatch(IAccountBillingEventConditionContext context);
    }

    public interface IAccountBillingEventConditionContext
    {
        Account Account { get; }

        dynamic BillingEvent { get; }
    }

    public class AccountBillingEventConditionContext : IAccountBillingEventConditionContext
    {
        long _accountId;
        public AccountBillingEventConditionContext(long accountId)
        {
            _accountId = accountId;
        }

        Account _account;
        public Account Account
        {
            get
            {
                if (_account == null)
                    _account = new AccountManager().GetAccount(_accountId);
                return _account;
            }
        }

        public dynamic BillingEvent { get; set; }
    }
}
