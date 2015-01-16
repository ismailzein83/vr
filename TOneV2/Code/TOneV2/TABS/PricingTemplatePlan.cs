using System.Collections.Generic;
using System.Text;


namespace TABS
{
    public partial class PricingTemplatePlan : Components.FlaggedServicesEntity, Interfaces.ICachedCollectionContainer
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(PricingTemplatePlan).FullName);
        }

        #region Static
        internal static Dictionary<int, PricingTemplatePlan> _All;

        /// <summary>
        /// All pricing  templates for all the services 
        /// </summary>
        public static Dictionary<int, PricingTemplatePlan> All
        {
            get
            {
                if (_All == null)
                {
                    _All = ObjectAssembler.GetAllPricingTemplatePlans();
                }
                return _All;
            }
        }
        #endregion Static

        #region DataMembers
        public virtual int PricingTemplatePlanId { get; set; }
        public virtual PricingTemplate PricingTemplate { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual double FromPrice { get; set; }
        public virtual double ToPrice { get; set; }
        public virtual decimal? Margin { get; set; }
        public virtual decimal? MaxMargin { get; set; }
        public virtual decimal? Fixed { get; set; }
        public virtual PricingReferenceRate PricingReferenceRate { get; set; }
        public virtual string Description { get; set; }
        public virtual CarrierAccount Supplier { get; set; }
        protected string _IsPerc;
        public virtual bool IsPerc
        {
            get { return "Y".Equals(_IsPerc); }
            set { _IsPerc = value ? "Y" : "N"; }
        }
        public override string Identifier
        {
            get { return "Pricing Template Plan:" + this.PricingTemplatePlanId.ToString(); }
        }
        protected string _NotContinues;
        public virtual bool NotContinues
        {
            get { return "Y".Equals(_NotContinues); }
            set { _NotContinues = value ? "Y" : "N"; }
        }
        public virtual int Priority { get; set; }

        public string DefinitionDisplay
        {

            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} : From {1} To {2}, Margin: {3}{4}, Currency:{5} , Not Continues:{6} ,Priority {7}",
                    this.PricingTemplate.Title, this.FromPrice.ToString(), this.ToPrice.ToString(), this.Margin.ToString(), IsPerc ? "%" : "", this.PricingTemplate.Currency.Symbol, this.NotContinues ? "Y" : "N", this.Priority);
                return sb.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            PricingTemplatePlan Other = obj as PricingTemplatePlan;
            return
             (this.PricingTemplatePlanId == Other.PricingTemplatePlanId);
        }

        public override int GetHashCode()
        {
            if (this.PricingTemplatePlanId == 0)
                return base.GetHashCode();
            else
                return this.PricingTemplatePlanId;
        }

        #endregion DataMembers


        #region IComparable<PricingTemplatePlan> Members

        public int CompareTo(PricingTemplatePlan other)
        {

            if (this.Priority > other.Priority)
            {
                return this.Priority;
            }
            else
            {
                return this.Priority.CompareTo(other.Priority);
            }
            //if (this.Priority.Equals(other.Priority))
            //    return this.Priority.CompareTo(other.Priority);
            //else
            //    return this.Carrier.Name.CompareTo(other.Carrier.Name);
        }



        #endregion
    }
}
