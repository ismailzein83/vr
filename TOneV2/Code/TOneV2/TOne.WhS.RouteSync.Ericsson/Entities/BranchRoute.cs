using System;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public class BranchRoute
    {
        public string Name { get; set; }
        public string AlternativeName { get; set; }
        public bool IncludeTrunkAsSwitch { get; set; }
        public bool OverflowOnFirstOptionOnly { get; set; }
    }

    public abstract class BaseBranchRoute
    {
        public abstract bool IncludeTrunkAsSwitch { get; set; }
        public abstract bool OverflowOnFirstOptionOnly { get; set; }
        public abstract BranchRoute GetBranchRoute();
    }

    public class RORangeBranchRoute : BaseBranchRoute
    {
        public int From { get; set; }
        public int? To { get; set; }
        public override bool IncludeTrunkAsSwitch { get; set; }
        public override bool OverflowOnFirstOptionOnly { get; set; }
        public override BranchRoute GetBranchRoute()
        {
            return new BranchRoute()
            {
                Name = (this.To.HasValue) ? string.Format("RO-{0}&&-{1}", this.From, this.To) : string.Format("RO-{0}", this.From),
                IncludeTrunkAsSwitch = this.IncludeTrunkAsSwitch,
                OverflowOnFirstOptionOnly = this.OverflowOnFirstOptionOnly
            };
        }
    }

    public class FreeTextBranchRoute : BaseBranchRoute
    {
        public string Name { get; set; }
        public string AlternativeName { get; set; }
        public override bool IncludeTrunkAsSwitch { get; set; }
        public override bool OverflowOnFirstOptionOnly { get; set; }
        public override BranchRoute GetBranchRoute()
        {
            return new BranchRoute()
            {
                Name = this.Name,
                AlternativeName = this.AlternativeName,
                IncludeTrunkAsSwitch = this.IncludeTrunkAsSwitch,
                OverflowOnFirstOptionOnly = this.OverflowOnFirstOptionOnly
            };
        }
    }
}
