using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DataRecordSearchPageSettings : AnalyticReportSettings
    {
        public override Guid ConfigId { get { return new Guid("82AA89F6-4D19-4168-A499-CDD2875F1702"); } }

        public List<DRSearchPageStorageSource> Sources { get; set; }

        public int? MaxNumberOfRecords { get; set; }

        public int NumberOfRecords { get; set; }

        public override bool DoesUserHaveAccess(Security.Entities.IViewUserAccessContext context)
        {
            IDataRecordStorageManager _genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IDataRecordStorageManager>();
            foreach (var r in this.Sources)
            {
                var fieldNames = r.GridColumns.MapRecords(x => x.FieldName);
                if (_genericBusinessEntityManager.DoesUserHaveAccess(context.UserId, r.RecordStorageIds) && _genericBusinessEntityManager.DoesUserHaveFieldsAccess(context.UserId, r.RecordStorageIds, fieldNames))
                    return true;
            }
            return false;
        }

        public override void ApplyTranslation(IAnalyticReportTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

            if (Sources != null && Sources.Count > 0)
            {
                foreach (var source in Sources)
                {
                    if (source.TitleResourceKey != null)
                        source.Title = vrLocalizationManager.GetTranslatedTextResourceValue(source.TitleResourceKey, source.Title, context.LanguageId);

                    if (source.GridColumns != null && source.GridColumns.Count > 0)
                    {
                        foreach (var gridColumn in source.GridColumns)
                        {
                            if (gridColumn.TitleResourceKey != null)
                                gridColumn.FieldTitle = vrLocalizationManager.GetTranslatedTextResourceValue(gridColumn.TitleResourceKey, gridColumn.FieldTitle, context.LanguageId);
                        }
                    }
                    if (source.Filters != null && source.Filters.Count > 0)
                    {
                        foreach (var filter in source.Filters)
                        {
                            if (filter.TitleResourceKey != null)
                                filter.FieldTitle = vrLocalizationManager.GetTranslatedTextResourceValue(filter.TitleResourceKey, filter.FieldTitle, context.LanguageId);
                        }
                    }
                    if (source.ItemDetails != null && source.ItemDetails.Count > 0)
                    {
                        foreach (var detail in source.ItemDetails)
                        {
                            if (detail.TitleResourceKey != null)
                                detail.FieldTitle = vrLocalizationManager.GetTranslatedTextResourceValue(detail.TitleResourceKey, detail.FieldTitle, context.LanguageId);
                        }
                    }
                    if (source.SubviewDefinitions != null && source.SubviewDefinitions.Count > 0)
                    {
                        foreach (var subview in source.SubviewDefinitions)
                        {
                            if (subview.NameResourceKey != null)
                                subview.Name = vrLocalizationManager.GetTranslatedTextResourceValue(subview.NameResourceKey, subview.Name, context.LanguageId);
                        }
                    }
                }
            }
        }
    }

    public class DRSearchPageStorageSource
    {
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }
        public string Name { get; set; }
        public Guid DataRecordTypeId { get; set; }
        public List<Guid> RecordStorageIds { get; set; }
        public List<DRSearchPageGridColumn> GridColumns { get; set; }
        public List<DRSearchPageItemDetail> ItemDetails { get; set; }
        public List<DRSearchPageSubviewDefinition> SubviewDefinitions { get; set; }
        public List<DRSearchPageSortColumn> SortColumns { get; set; }
        public List<DRSearchPageFilter> Filters { get; set; }
        public RecordFilterGroup RecordFilter { get; set; }
        public bool HideTimeRange { get; set; }
    }

    public class DRSearchPageFilter
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public string TitleResourceKey { get; set; }
        public Boolean IsRequired { get; set; }
    }

    public class DRSearchPageGridColumn
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public string TitleResourceKey { get; set; }
        public GridColumnSettings ColumnSettings { get; set; }
        public bool IsHidden { get; set; }
        public Guid? ColumnStyleId { get; set; }
    }

    public class DRSearchPageItemDetail
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public string TitleResourceKey { get; set; }
        public int ColumnWidth { get; set; }
    }

    public class DRSearchPageSortColumn
    {
        public string FieldName { get; set; }
        public bool IsDescending { get; set; }
    }

    public class DRSearchPageSubviewDefinition
    {
        public Guid SubviewDefinitionId { get; set; }
        public string Name { get; set; }
        public string NameResourceKey { get; set; }
        public DRSearchPageSubviewDefinitionSettings Settings { get; set; }
    }

    public abstract class DRSearchPageSubviewDefinitionSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
    }
}