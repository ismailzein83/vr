using System;

namespace TABS
{
    [Serializable]
    public class PostpaidAmount : PrePostTransaction
    {
        public override string ToString()
        {
            return string.Format("Postpaid {0} - Reference : {1}, Type : {2}, Amount : {3}, Currency : {4}", CarrierName, ReferenceNumber, Type, Amount, Currency);
        }

        public override string Identifier { get { return "Postpaid Amount:" + ID; } }
    }
}
