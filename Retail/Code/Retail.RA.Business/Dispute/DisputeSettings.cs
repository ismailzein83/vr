using System;
using Retail.BusinessEntity.Business;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.RA.Business
{
    public class DisputeSettings : GenericBEExtendedSettings
    {
        public override Guid ConfigId => new Guid("7D1E3160-A230-4A67-BD35-9221D03A0270");

        public override object GetInfoByType(IGenericBEExtendedSettingsContext context)
        {
            if (context.InfoType == null)
                return null;

            switch (context.InfoType)
            {
                case "SerialNumberPattern": return new ConfigManager().GetDisputeSerialNumberPattern();

                case "SerialNumberInitialSequence": return new ConfigManager().GetDisputeSerialNumberInitialSequence();

                case "OpenDisputeMailTemplate": return new ConfigManager().GetDisputeOpenMailTemplateId();

                case "PendingDisputeMailTemplate": return new ConfigManager().GetDisputePendingMailTemplateId();

                case "ClosedDisputeMailTemplate": return new ConfigManager().GetDisputeClosedMailTemplateId();

                case "RAOperator":
                    var operatorId = Convert.ToInt32(context.GenericBusinessEntity.FieldValues.GetRecord("OperatorId"));
                    Guid accountBedDefinitionId = new Guid("1A4A2877-D4C0-4B97-B4F0-2942BA342485");
                    var account = new AccountBEManager().GetAccount(accountBedDefinitionId, operatorId);
                    return account;

                default: return null;
            }
        }
    }
}
