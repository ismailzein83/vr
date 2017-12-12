using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class GeneralSettingData :SettingData
    {
        public UISettingData UIData { get; set; }

        public CacheSettingData CacheData { get; set; }
    }
    public class UISettingData
    {
        public Guid? DefaultViewId { get; set; }

        public int? NormalPrecision { get; set; }

        public int? LongPrecision { get; set; }

        public int? GridPageSize { get; set; }

        public long MaxSearchRecordCount { get; set; }
        public Guid? DefaultLanguageId { get; set; }
    }

    public class CacheSettingData
    {
        public long ClientCacheNumber { get; set; }
    }
    public class GeneralTechnicalSettingData :SettingData
    {

        public CompanySettingDefinition CompanySettingDefinition { get; set; }

        public bool IsLocalizationEnabled { get; set; }
    }

    public class GoogleAnalyticsData : SettingData
    {
        public bool IsEnabled { get; set; }

        public string Account { get; set; }
    }

    public class CompanySettingDefinition
    {
        public List<CompanyContactType> ContactTypes { get; set; }
        public Dictionary<Guid, CompanyDefinitionSetting> ExtendedSettings { get; set; }

    }
    public class CompanyDefinitionSetting
    {
        public Guid CompanyDefinitionSettingId { get; set; }
        public string Name  { get; set; }
        public BaseCompanyDefinitionExtendedSetting Setting { get; set; }
    }
    public abstract class  BaseCompanyDefinitionExtendedSetting
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
    }
    public class CompanyContactType
    {
        public string Name { get; set; }

        public string Title { get; set; }
    }
}
