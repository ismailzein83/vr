using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
	public sealed class SetRatePlanContext : CodeActivity
	{
		protected override void CacheMetadata(CodeActivityMetadata metadata)
		{
			metadata.AddDefaultExtensionProvider<IRatePlanContext>(() => new RatePlanContext());
			base.CacheMetadata(metadata);
		}

		protected override void Execute(CodeActivityContext context)
		{

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

		public DateTime RetroactiveDate
		{
			get
			{
				return _retroactiveDate;
			}
		}
	}
}
