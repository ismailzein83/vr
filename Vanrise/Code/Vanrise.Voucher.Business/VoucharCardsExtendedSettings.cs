using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;

namespace Vanrise.Voucher.Business
{
    public class VoucharCardsExtendedSettings : GenericBEExtendedSettings
    {
        public List<VoucharCardSerialNumberPart> SerialNumberParts { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("6B038908-4F30-4F0F-8E05-E13FE5577A0D");}
        }

    }
    public class VoucharCardSerialNumberPart
    {
        public string VariableName { get; set; }
        public string Description { get; set; }
        public VRConcatenatedPartSettings<IVoucharCardSerialNumberPartConcatenatedPartContext> Settings { get; set; }

    }
    public interface IVoucharCardSerialNumberPartConcatenatedPartContext
    {
        Guid VoucherCardBEDefinitionId { get; set; }
    }

    public class VoucharCardDateSerialNumberPart : VRConcatenatedPartSettings<IVoucharCardSerialNumberPartConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return new Guid("71DC99F2-0A5D-47B6-9FB5-419E7F936017"); } }
        public string DateFormat { get; set; }
        public override string GetPartText(IVoucharCardSerialNumberPartConcatenatedPartContext context)
        {
            var date = DateTime.Now;
            if (!String.IsNullOrEmpty(this.DateFormat))
            {
                return date.ToString(this.DateFormat);
            }
            return date.ToString();
        }
    }

    public enum DateCounterType { Yearly = 0 }
    public class VoucharCardSequenceSerialNumberPart : VRConcatenatedPartSettings<IVoucharCardSerialNumberPartConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return new Guid("F8F17A71-DDF2-4D7E-873F-861A79D67793"); } }
        public DateCounterType? DateCounterType { get; set; }
        public int PaddingLeft { get; set; }
        public override string GetPartText(IVoucharCardSerialNumberPartConcatenatedPartContext context)
        {
            StringBuilder sequenceKey = new StringBuilder();
            StringBuilder sequenceGroup = new StringBuilder();
            sequenceGroup.Append("OVERALL");

            long initialSequenceValue = new ConfigManager().GetSerialNumberPartInitialSequence();

            if (this.DateCounterType.HasValue)
            {
                if (sequenceKey.Length > 0)
                    sequenceKey.Append("_");
                sequenceGroup.Append("_");
                sequenceGroup.Append(Common.Utilities.GetEnumDescription(this.DateCounterType.Value));
                switch (this.DateCounterType)
                {
                    case Vanrise.Voucher.Business.DateCounterType.Yearly:
                        sequenceKey.Append(string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.Year + 1));
                        break;
                }
            }
            VRSequenceManager manager = new VRSequenceManager();
            var sequenceNumber = manager.GetNextSequenceValue(sequenceGroup.ToString(), context.VoucherCardBEDefinitionId, sequenceKey.ToString(), initialSequenceValue);
            return sequenceNumber.ToString().PadLeft(this.PaddingLeft, '0');
        }
    }
}
