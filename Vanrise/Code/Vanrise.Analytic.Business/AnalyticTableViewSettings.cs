using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.Business
{
	public class AnalyticTableViewSettings:ViewSettings
	{
		public Guid AnalyticTableId { get; set; }
		public virtual string GetURL(View view)
		{
			return view.Url;
		}
		public override bool DoesUserHaveAccess(IViewUserAccessContext context)
		{
			AnalyticTableManager _analytictableManager = new AnalyticTableManager();
			return _analytictableManager.DoesUserHaveAccess(context.UserId, AnalyticTableId);
		}
	}
}
