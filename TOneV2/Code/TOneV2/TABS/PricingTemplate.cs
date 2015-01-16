using System.Collections.Generic;
using System.Text;

namespace TABS
{
    public class PricingTemplate : Interfaces.ICachedCollectionContainer
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(PricingTemplate).FullName);
        }

        #region Static
        internal static Dictionary<int, PricingTemplate> _All;

        /// <summary>
        /// All pricing  templates for all the services 
        /// </summary>
        public static Dictionary<int, PricingTemplate> All
        {
            get
            {
                if (_All == null)
                {
                    _All = ObjectAssembler.GetAllPricingTemplate();
                }
                return _All;
            }
        }

        #endregion Static

        #region DataMembers

        public virtual int PricingTemplateId { get; set; }
        public virtual string Title { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual string Description { get; set; }
        public virtual TemplateType TemplateType { get; set; }
        private IList<PricingTemplatePlan> _PricingTemplatePlans;

        public virtual IList<PricingTemplatePlan> PricingTemplatePlans
        {
            get
            {
                if (_PricingTemplatePlans == null)
                {
                    _PricingTemplatePlans = ObjectAssembler.GetPricingTemplatePlans(this);
                }
                return _PricingTemplatePlans;
            }
        }
        public string DefinitionDisplay
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} ({1})",
                   this.Title,
                   this.Currency.Symbol
                   );
                return sb.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            PricingTemplate Other = obj as PricingTemplate;
            return
             (this.PricingTemplateId == Other.PricingTemplateId);
        }

        public override int GetHashCode()
        {
            if (this.Title == null)
                return base.GetHashCode();
            else
                return this.Title.GetHashCode();
        }

        #endregion DataMembers

    }
}
