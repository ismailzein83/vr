using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
	public sealed class SetImportSPLContext : CodeActivity
	{
		protected override void CacheMetadata(CodeActivityMetadata metadata)
		{
			metadata.AddDefaultExtensionProvider<IImportSPLContext>(() => new ImportSPLContext());
			base.CacheMetadata(metadata);
		}

		protected override void Execute(CodeActivityContext context)
		{

		}
	}

	internal static class ContextExtensionMethods
	{
		public static IImportSPLContext GetSPLParameterContext(this ActivityContext context)
		{
			var parameterContext = context.GetExtension<IImportSPLContext>();
			if (parameterContext == null)
				throw new NullReferenceException("parameterContext");
			return parameterContext;
		}
	}

	public class ImportSPLContext : IImportSPLContext
	{
		private object _obj = new object();

		private volatile bool _processHasChanges = false;

		private TimeSpan _codeCloseDateOffset;

		public const string CustomDataKey = "ImportSPLContext";

		public ImportSPLContext()
		{
			int effectiveDateDayOffset = new TOne.WhS.BusinessEntity.Business.ConfigManager().GetPurchaseAreaEffectiveDateDayOffset();
			_codeCloseDateOffset = new TimeSpan(effectiveDateDayOffset, 0, 0, 0);
		}

		public bool ProcessHasChanges
		{
			get
			{
				return this._processHasChanges;
			}
		}

		public TimeSpan CodeCloseDateOffset
		{
			get
			{
				return _codeCloseDateOffset;
			}
		}

		public void SetToTrueProcessHasChangesWithLock()
		{
			if (!_processHasChanges)
			{
				lock (_obj)
				{
					this._processHasChanges = true;
				}
			}
		}
	}
}
