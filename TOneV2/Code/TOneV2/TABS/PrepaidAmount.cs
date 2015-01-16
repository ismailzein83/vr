using System;

namespace TABS
{
    [Serializable]
    public class PrepaidAmount : PrePostTransaction
    {
        public override string ToString()
        {
            return string.Format("Prepaid {0} - Reference : {1}, Type : {2}, Amount : {3}, Currency : {4}", CarrierName, ReferenceNumber, Type, Amount, Currency);
        }

        public override string Identifier { get { return "Prepaid Amount:" + ID; } }
    }
}
