using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Business;

namespace TOne.WhS.Sales.BP.Activities
{
	public sealed class SetRatePlanContext : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

		[RequiredArgument]
		public InArgument<int> OwnerId { get; set; }

		[RequiredArgument]
		public InArgument<DateTime> EffectiveDate { get; set; }

		#endregion

		protected override void CacheMetadata(CodeActivityMetadata metadata)
		{
			metadata.AddDefaultExtensionProvider<IRatePlanContext>(() => new RatePlanContext());
			base.CacheMetadata(metadata);
		}

		protected override void Execute(CodeActivityContext context)
		{
			SalePriceListOwnerType ownerType = OwnerType.Get(context);
			int ownerId = OwnerId.Get(context);
			DateTime effectiveDate = EffectiveDate.Get(context);

			RatePlanContext ratePlanContext = context.GetRatePlanContext() as RatePlanContext;
			ratePlanContext.OwnerType = ownerType;
			ratePlanContext.OwnerId = ownerId;
			ratePlanContext.EffectiveDate = effectiveDate;
			ratePlanContext.RateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveDate));
		}
	}

	internal static class ContextExtensionMethods
	{
		public static IRatePlanContext GetRatePlanContext(this ActivityContext context)
		{
			IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();
			if (ratePlanContext == null)
				throw new NullReferenceException("ratePlanContext");
			return ratePlanContext;
		}
	}
}
