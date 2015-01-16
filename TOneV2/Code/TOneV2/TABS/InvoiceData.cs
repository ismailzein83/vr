using System;

namespace TABS
{
    [Serializable]
    public class InvoiceData
    {
        public virtual int ID { get; set; }
        public virtual byte[] SourceFileBytes { get; set; }
    }
}
