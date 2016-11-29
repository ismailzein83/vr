using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
	public sealed class SetRatePlanContext : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<TOne.WhS.BusinessEntity.Entities.SalePriceListOwnerType> OwnerType { get; set; }

		#endregion

		protected override void CacheMetadata(CodeActivityMetadata metadata)
		{
			metadata.AddDefaultExtensionProvider<IRatePlanContext>(() => new RatePlanContext());
			base.CacheMetadata(metadata);
		}

		protected override void Execute(CodeActivityContext context)
		{
			SalePriceListOwnerType ownerType = OwnerType.Get(context);
			RatePlanContext ratePlanContext = context.GetRatePlanContext() as RatePlanContext;
			ratePlanContext.OwnerType = ownerType;
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

	public class RatePlanContext : IRatePlanContext
	{
		private DateTime _retroactiveDate;

		public RatePlanContext()
		{
			var ratePlanManager = new RatePlanManager();
			TOne.WhS.BusinessEntity.Entities.SaleAreaSettingsData saleAreaSettings = ratePlanManager.GetSaleAreaSettingsData();
			_retroactiveDate = DateTime.Now.Date.AddDays(-saleAreaSettings.RetroactiveDayOffset);
		}

		public SalePriceListOwnerType OwnerType { get; set; }
		public DateTime RetroactiveDate
		{
			get
			{
				return _retroactiveDate;
			}
		}
	}
}
