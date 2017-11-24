using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SalePriceListTemplateManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SalePriceListTemplateDetail> GetFilteredSalePriceListTemplates(Vanrise.Entities.DataRetrievalInput<SalePriceListTemplateQuery> input)
        {
            Dictionary<int, SalePriceListTemplate> cachedSalePriceListTemplates = GetCachedSalePriceListTemplates();
            Func<SalePriceListTemplate, bool> filterFunc = (salePriceListTemplate) => (input.Query.Name == null || salePriceListTemplate.Name.ToLower().Contains(input.Query.Name.ToLower()));

            ResultProcessingHandler<SalePriceListTemplateDetail> handler = new ResultProcessingHandler<SalePriceListTemplateDetail>()
            {
                ExportExcelHandler = new SalePriceListTemplateExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(SalePriceListTemplateLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedSalePriceListTemplates.ToBigResult(input, filterFunc, SalePriceListTemplateDetailMapper), handler);
        }
        public SalePriceListTemplate GetSalePriceListTemplateHistoryDetailbyHistoryId(int salePriceListTemplateHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var salePriceListTemplate = s_vrObjectTrackingManager.GetObjectDetailById(salePriceListTemplateHistoryId);
            return salePriceListTemplate.CastWithValidate<SalePriceListTemplate>("SalePriceListTemplate : historyId ", salePriceListTemplateHistoryId);
        }
        public SalePriceListTemplate GetSalePriceListTemplate(int salePriceListTemplateId, bool isViewedFromUI)
        {
            var salePriceListTemplateItem = GetCachedSalePriceListTemplates().GetRecord(salePriceListTemplateId);
            if (salePriceListTemplateItem != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(SalePriceListTemplateLoggableEntity.Instance, salePriceListTemplateItem);
            return salePriceListTemplateItem;
        }

        public SalePriceListTemplate GetSalePriceListTemplate(int salePriceListTemplateId)
        {
            return GetSalePriceListTemplate(salePriceListTemplateId, false);
        }
        public IEnumerable<SalePriceListTemplateInfo> GetSalePriceListTemplatesInfo()
        {
            return GetCachedSalePriceListTemplates().MapRecords(SalePriceListTemplateInfoMapper).OrderBy(x => x.Name);
        }

        public IEnumerable<SalePricelistTemplateSettingsMappedCellExtensionConfig> GetMappedCellsExtensionConfigs()
        {
            var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<SalePricelistTemplateSettingsMappedCellExtensionConfig>(SalePricelistTemplateSettingsMappedCellExtensionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<SalePricelistTemplateSettingsExtensionConfig> GetSalePricelistTemplateSettingsExtensionConfigs()
        {
            var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<SalePricelistTemplateSettingsExtensionConfig>(SalePricelistTemplateSettingsExtensionConfig.EXTENSION_TYPE);
        }

        public string GetSalePriceListTemplateName(SalePriceListTemplate salePriceListTemplate)
        {
            return (salePriceListTemplate != null) ? salePriceListTemplate.Name : null;
        }

        public IEnumerable<SalePriceListTemplateSettingsMappedTableExtensionConfig> GetMappedTablesExtensionConfigs()
        {
            var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<SalePriceListTemplateSettingsMappedTableExtensionConfig>(SalePriceListTemplateSettingsMappedTableExtensionConfig.EXTENSION_TYPE);
        }
        public IEnumerable<BasicMappedValueExtensionConfig> GetBasicSettingsMappedValueExtensionConfigs(string configType)
        {
            var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<BasicMappedValueExtensionConfig>(configType);
        }

        public Vanrise.Entities.InsertOperationOutput<SalePriceListTemplateDetail> AddSalePriceListTemplate(SalePriceListTemplate salePriceListTemplate)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SalePriceListTemplateDetail>();
            insertOperationOutput.InsertedObject = null;
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;

            var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListTemplateDataManager>();
            int insertedId;
            if (salePriceListTemplate.Settings != null)
            {
                var context = new PriceListTemplateOnBeforeSaveContext() { SaveOperationType = SaveOperationType.Insert };
                salePriceListTemplate.Settings.OnBeforeSave(context);
            }
            if (dataManager.Insert(salePriceListTemplate, out insertedId))
            {
                if (salePriceListTemplate.Settings != null)
                {
                    var context = new PriceListTemplateOnAfterSaveContext() { SaveOperationType = SaveOperationType.Insert, TemplateId = insertedId };
                    salePriceListTemplate.Settings.OnAfterSave(context);
                }
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                salePriceListTemplate.SalePriceListTemplateId = insertedId;
                VRActionLogger.Current.TrackAndLogObjectAdded(SalePriceListTemplateLoggableEntity.Instance, salePriceListTemplate);
                insertOperationOutput.InsertedObject = SalePriceListTemplateDetailMapper(salePriceListTemplate);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SalePriceListTemplateDetail> UpdateSalePriceListTemplate(SalePriceListTemplate salePriceListTemplate)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SalePriceListTemplateDetail>();
            updateOperationOutput.UpdatedObject = null;
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            if (salePriceListTemplate.Settings != null)
            {
                var context = new PriceListTemplateOnBeforeSaveContext() { SaveOperationType = SaveOperationType.Update, TemplateId = salePriceListTemplate.SalePriceListTemplateId};
                salePriceListTemplate.Settings.OnBeforeSave(context);
            }

            var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListTemplateDataManager>();

            if (dataManager.Update(salePriceListTemplate))
            {
                if (salePriceListTemplate.Settings != null)
                {
                    var context = new PriceListTemplateOnAfterSaveContext();
                    salePriceListTemplate.Settings.OnAfterSave(context);
                }
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(SalePriceListTemplateLoggableEntity.Instance, salePriceListTemplate);
                updateOperationOutput.UpdatedObject = SalePriceListTemplateDetailMapper(salePriceListTemplate);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Classes

        private class SalePriceListTemplateExcelExportHandler : ExcelExportHandler<SalePriceListTemplateDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SalePriceListTemplateDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Sale Pricelist",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISalePriceListTemplateDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreSalePriceListTemplatesUpdated(ref _updateHandle);
            }
        }

        private class SalePriceListTemplateLoggableEntity : VRLoggableEntityBase
        {
            public static SalePriceListTemplateLoggableEntity Instance = new SalePriceListTemplateLoggableEntity();

            private SalePriceListTemplateLoggableEntity()
            {

            }

            static SalePriceListTemplateManager s_salePriceListTemplateManager = new SalePriceListTemplateManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SalePriceListTemplate"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Sale Price List Template"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SalePriceListTemplate_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SalePriceListTemplate salePriceListTemplate = context.Object.CastWithValidate<SalePriceListTemplate>("context.Object");
                return salePriceListTemplate.SalePriceListTemplateId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                SalePriceListTemplate salePriceListTemplate = context.Object.CastWithValidate<SalePriceListTemplate>("context.Object");
                return s_salePriceListTemplateManager.GetSalePriceListTemplateName(salePriceListTemplate);

            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, SalePriceListTemplate> GetCachedSalePriceListTemplates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSalePriceListTemplates", () =>
            {
                ISalePriceListTemplateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListTemplateDataManager>();
                IEnumerable<SalePriceListTemplate> salePriceListTemplates = dataManager.GetAll();
                return salePriceListTemplates.ToDictionary(x => x.SalePriceListTemplateId);
            });
        }

        private IEnumerable<SalePLZoneNotification> GetZoneNotifications()
        {
            var zones = new List<SalePLZoneNotification>();
            for (int i = 0; i < 3; i++)
            {
                var zone = new SalePLZoneNotification()
                {
                    ZoneName = "Zone " + (i + 1)
                };

                zone.Codes.Add(new SalePLCodeNotification()
                {
                    Code = "Code " + (i + 1),
                    BED = DateTime.Now.Date
                });

                zone.Rate = new SalePLRateNotification()
                {
                    Rate = (i + 1),
                    BED = DateTime.Now.Date
                };

                zones.Add(zone);
            }
            return zones;
        }

        #endregion

        #region Mappers

        private SalePriceListTemplateDetail SalePriceListTemplateDetailMapper(SalePriceListTemplate salePriceListTemplate)
        {
            return new SalePriceListTemplateDetail()
            {
                Entity = salePriceListTemplate
            };
        }

        private SalePriceListTemplateInfo SalePriceListTemplateInfoMapper(SalePriceListTemplate entity)
        {
            return new SalePriceListTemplateInfo()
            {
                SalePriceListTemplateId = entity.SalePriceListTemplateId,
                Name = entity.Name
            };
        }

        #endregion
    }

    public class PriceListTemplateOnBeforeSaveContext : IPriceListTemplateOnBeforeSaveContext
    {
        public SaveOperationType SaveOperationType { get; set; }
        public int? TemplateId { get; set; }
    }

    public class PriceListTemplateOnAfterSaveContext : IPriceListTemplateOnAfterSaveContext
    {
        public SaveOperationType SaveOperationType { get; set; }
        public int TemplateId { get; set; }
    }

    public class BasicSalePricelistTemplateFileSettings : VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7949D926-396C-4BD0-A2A5-2FA86A236A9C"); }
        }

        public int? SalePricelistTemplateId { get; set; }

        Vanrise.Security.Business.SecurityManager s_securityManager = new Vanrise.Security.Business.SecurityManager();
        public override bool DoesUserHaveViewAccess(Vanrise.Entities.IVRFileDoesUserHaveViewAccessContext context)
        {
            return s_securityManager.HasPermissionToActions("WhS_BE/SalePriceListTemplate/GetFilteredSalePriceListTemplates", context.UserId);
        }
    }
}
