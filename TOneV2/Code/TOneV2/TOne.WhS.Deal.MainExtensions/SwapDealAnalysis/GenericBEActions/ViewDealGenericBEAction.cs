using System;
using Vanrise.GenericData.Business;

namespace TOne.WhS.Deal.MainExtensions
{
    public class ViewSwapDealGenericBEAction : GenericBEActionSettings
    {
        public override string ActionKind => "ViewSwapDeal";

        public override string ActionTypeName => "ViewSwapDealGenericBEAction";

        public override Guid ConfigId => new Guid("8493394D-6B95-4870-92D3-AF81169BC8D4");
    }
}
