using System;

namespace TABS
{
    [Serializable]
    public class PriceListData
    {
        public virtual int ID { get; set; }
        public byte[] SourceFileBytes { get; set; }
    }
}
