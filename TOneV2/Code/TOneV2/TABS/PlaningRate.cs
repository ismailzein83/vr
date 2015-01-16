
namespace TABS
{
	public class PlaningRate : RateBase
	{
        public override string Identifier { get { return "PlanningRate:" + ID; } }

		public virtual RatePlan RatePlan
		{
			get
			{
				return (RatePlan)base.PriceListBase;
			}
			set
			{
				base.PriceListBase = value;
			}
		}
        private string _MarginStates = "";
        public virtual string MarginState
        {
            get { return _MarginStates; }
            set { _MarginStates = value; }
        }
        
	}
}