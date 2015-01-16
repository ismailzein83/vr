using System;

namespace TABS
{
    [Serializable]
    public class PriceList : PriceListBase
    {
        public override string Identifier { get { return "PriceList:" + ID; } }

        /// <summary>
        /// The pricelist that does not exist
        /// </summary>
        public static PriceList None = new PriceList(int.MinValue);

        public PriceListData Data { get; protected set; }
        
        public virtual byte[] SourceFileBytes
        {
            get 
            {
                lock(this)
                {
                    if (ID > 0 && this.Data == null)
                        this.Data = TABS.ObjectAssembler.Get<PriceListData>(this.ID);
                    if (this.Data == null)
                        this.Data = new PriceListData();
                }
                return this.Data.SourceFileBytes; 
            }
            set 
            {
                if (this.Data == null)
                    this.Data = new PriceListData();
                this.Data.SourceFileBytes = value; 
            }
        }    

        protected PriceList(int id)
            : base(id)
        {
        }

        public PriceList()
        {

        }

        public void FixRateChanges()
        {
            // Fix the rate changes for this pricelist
            DataHelper.FixRateChanges(this);
            // Reload rates
            this.Rates = null;
        }
    }
}