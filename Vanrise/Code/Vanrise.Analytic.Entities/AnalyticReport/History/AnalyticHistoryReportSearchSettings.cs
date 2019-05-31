﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticHistoryReportSearchSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual void ApplyTranslation(IAnalyticHistoryReportTranslationContext context)
        {


        }
    }
    public interface IAnalyticHistoryReportTranslationContext
    {
         Guid LanguageId { get;}
         Guid AnalyticTableId { get;}
    }
    public class AnalyticHistoryReportTranslationContext: IAnalyticHistoryReportTranslationContext
    {
        public Guid LanguageId { get; set; }
        public Guid AnalyticTableId { get; set; }
    }
}
