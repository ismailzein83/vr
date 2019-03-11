using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Vanrise.MobileNetwork.Business
{
    public class NumberPrefixOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("87F57377-6780-43E4-B8EB-BEA029DC01B3"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            List<string> overlappingNetworkNames = new List<string>();
            NumberPrefixManager numberPrefixManager = new NumberPrefixManager();
            MobileNetworkManager mobileNetworkManager = new MobileNetworkManager();

            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            var numberPrefixId = context.GenericBusinessEntity.FieldValues.GetRecord("ID");
            var code = context.GenericBusinessEntity.FieldValues.GetRecord("Code");
            code.ThrowIfNull("code");
            var bed = context.GenericBusinessEntity.FieldValues.GetRecord("BED");
            bed.ThrowIfNull("BED");
            var eed = context.GenericBusinessEntity.FieldValues.GetRecord("EED");
            var mobileNetworkId = context.GenericBusinessEntity.FieldValues.GetRecord("MobileNetworkId");
            mobileNetworkId.ThrowIfNull("mobileNetworkId");

            var numberPrefixesWithSameCode = numberPrefixManager.GetNumberPrefixesByCode((string)code);
            if (numberPrefixesWithSameCode != null)
            {
                foreach (var numberPrefix in numberPrefixesWithSameCode)
                {
                    if (Utilities.AreTimePeriodsOverlapped((DateTime)bed, (DateTime?)eed, numberPrefix.BED, numberPrefix.EED) && (numberPrefixId == null || numberPrefix.Id != (long)numberPrefixId))
                    {
                        var mobileNetwork = mobileNetworkManager.GetMobileNetworkById(numberPrefix.MobileNetworkId);
                        overlappingNetworkNames.Add(mobileNetwork.NetworkName);
                    }
                }
            }
            if (overlappingNetworkNames.Count() > 0)
            {
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add(string.Format("The code Added is overlapping with codes in the following mobile network(s) : '{0}'",string.Join(",", overlappingNetworkNames)));
            }
        }
    }
}
