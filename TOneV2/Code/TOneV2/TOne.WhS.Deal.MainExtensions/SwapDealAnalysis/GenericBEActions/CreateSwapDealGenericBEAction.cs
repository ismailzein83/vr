using System;
using Vanrise.GenericData.Business;

namespace TOne.WhS.Deal.MainExtensions
{
    public class CreateSwapDealGenericBEAction : GenericBEActionSettings
    {
        public override string ActionKind => "CreateSwapDeal";

        public override string ActionTypeName => "CreateSwapDealGenericBEAction";

        public override Guid ConfigId => new Guid("E5666599-FF4C-423E-BC6A-724B4A68CA69");
    }
}
