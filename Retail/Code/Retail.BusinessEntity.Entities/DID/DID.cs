using System;
using System.Linq;
using Vanrise.Common;

namespace Retail.BusinessEntity.Entities
{
    public enum DIDNumberType { Number = 0, Range = 1, Prefix = 2 }

    public class DID
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_DID";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("674F8BE5-9F1B-4084-8EE7-4EBE6C8838AE");

        public int DIDId { get; set; }

        public string SourceId { get; set; }
        public DIDSettings Settings { get; set; }

        DIDNumberType? _didNumberType;
        string _description;

        public DIDNumberType DIDNumberType
        {
            get
            {
                if (!_didNumberType.HasValue)
                {
                    if (Settings != null)
                    {
                        if (Settings.Numbers != null && Settings.Numbers.Count > 0)
                            _didNumberType = Entities.DIDNumberType.Number;

                        if (Settings.Ranges != null && Settings.Ranges.Count > 0)
                            _didNumberType = Entities.DIDNumberType.Range;
                    }

                    if (!_didNumberType.HasValue)
                        throw new Exception("Invalid Type for DID.");
                }
                return _didNumberType.Value;
            }
        }

        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    switch (DIDNumberType)
                    {
                        case Entities.DIDNumberType.Number:
                            _description = string.Format("{0}: {1}", Settings.Numbers.Count > 1 ? "Numbers" : "Number", string.Join<string>(", ", Settings.Numbers));
                            break;
                        case Entities.DIDNumberType.Range:
                            _description = string.Format("{0}: {1}", Settings.Ranges.Count > 1 ? "Ranges" : "Range", string.Join<string>(", ", Settings.Ranges.Select(itm => string.Format("{0} -> {1}", itm.From, itm.To))));
                            break;
                        default: throw new Exception("Invalid Type for DID.");
                    }
                }
                return _description;
            }
        }

        public bool ContainNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
                return true;

            if (Settings == null)
                return false;

            string lowerNumber = number.ToLower();

            if (Settings.Numbers != null && Settings.Numbers.Count > 0)
            {
                if (Settings.Numbers.Select(itm => itm.ToLower()).Contains(lowerNumber))
                    return true;
            }

            if (Settings.Ranges != null && Settings.Ranges.Count > 0)
            {
                long numberAsLong = number.TryParseWithValidate<long>(long.TryParse);
                foreach (DIDRange range in Settings.Ranges)
                {
                    long from = range.From.TryParseWithValidate<long>(long.TryParse);
                    long to = range.To.TryParseWithValidate<long>(long.TryParse);
                    if (numberAsLong >= from && numberAsLong <= to)
                        return true;
                }
            }

            return false;
        }
    }
}
