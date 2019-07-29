using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class ListGenericFilterDefinitionSettings : GenericBEFilterDefinitionSettings
    {
        public List<GenericFilterDefinitionSettings> Filters { get; set; }

        public override Guid ConfigId { get { return new Guid("F5EC6398-6E04-4E2F-8CAB-4DF004F9839E"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-genericbe-listfilterruntime-generic"; } }
        public override void ApplyTranslation(IGenericBETranslationContext context)
        {
            if (Filters != null)
            {
                foreach (var filter in Filters)
                {
                    filter.ApplyTranslation(context);
                }
            }
        }
    }
}
