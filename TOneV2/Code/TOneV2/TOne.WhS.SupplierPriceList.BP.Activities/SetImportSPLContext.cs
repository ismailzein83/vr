using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
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
        public TimeSpan CodeCloseDateOffset
        {
            get { return new TimeSpan(7, 0, 0,0); }
        }
    }
}
