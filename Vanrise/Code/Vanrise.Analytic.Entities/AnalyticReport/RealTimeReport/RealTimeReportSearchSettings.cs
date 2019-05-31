using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class RealTimeReportSearchSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual void ApplyTranslation(IAnalyticRealTimeReportTranslationContext context) { }

    }
    public interface IAnalyticRealTimeReportTranslationContext
    {
        Guid LanguageId { get;}
    }
    public class AnalyticRealTimeReportTranslationContext : IAnalyticRealTimeReportTranslationContext
    {
        public Guid LanguageId { get; set; }
    }
}
