﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Invoice.Data;
using Vanrise.Caching;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Common.Business;
namespace Vanrise.Invoice.Business
{
    public class InvoiceSettingManager : IInvoiceSettingManager
    {
        #region Public Methods
        public InvoiceSetting GetInvoiceSetting(Guid invoiceSettingId, bool isViewedFromUI)
        {
            var invoiceSettings = GetCachedInvoiceSettings();
            var invoiceSetting= invoiceSettings.GetRecord(invoiceSettingId);
            if (invoiceSetting != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(new InvoiceSettingLoggableEntity(invoiceSetting.InvoiceTypeId), invoiceSetting);
            return invoiceSetting;
        }

        public InvoiceSetting GetInvoiceSetting(Guid invoiceSettingId)
        {
           return  GetInvoiceSetting(invoiceSettingId,false);
        }
        public IDataRetrievalResult<InvoiceSettingDetail> GetFilteredInvoiceSettings(DataRetrievalInput<InvoiceSettingQuery> input)
        {
            var allItems = GetCachedInvoiceSettings();

            Func<InvoiceSetting, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 && (input.Query.InvoiceTypeId == itemObject.InvoiceTypeId);

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, InvoiceSettingDetailMapper));
        }
        public bool DoesUserHaveAssignPartnerAccess(Guid invoiceSettingsId)
        {
            var settings = GetInvoiceSetting(invoiceSettingsId);            
            var invoiceType = (settings != null) ? new InvoiceTypeManager().GetInvoiceType(settings.InvoiceTypeId):null;
            if ( invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.Security != null && invoiceType.Settings.Security.AssignPartnerRequiredPermission != null)
                return DoesUserHaveAccess(invoiceType.Settings.Security.AssignPartnerRequiredPermission);
           
            return true;
        }
        public Vanrise.Entities.InsertOperationOutput<InvoiceSettingDetail> AddInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<InvoiceSettingDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            var invoicesettings = GetInvoiceSettings(invoiceSetting.InvoiceTypeId);
            if (invoicesettings == null || invoicesettings.Count() == 0)
            {
                invoiceSetting.IsDefault = true;
            }
            IInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();
            invoiceSetting.InvoiceSettingId = Guid.NewGuid();
            if (dataManager.InsertInvoiceSetting(invoiceSetting))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(new InvoiceSettingLoggableEntity(invoiceSetting.InvoiceTypeId), invoiceSetting);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = InvoiceSettingDetailMapper(invoiceSetting);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
    
        public Vanrise.Entities.UpdateOperationOutput<InvoiceSettingDetail> UpdateInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<InvoiceSettingDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();

            if (dataManager.UpdateInvoiceSetting(invoiceSetting))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(new InvoiceSettingLoggableEntity(invoiceSetting.InvoiceTypeId), invoiceSetting);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = InvoiceSettingDetailMapper(invoiceSetting);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public string GetInvoiceSettingName(Guid invoiceSettingId)
        {
            var invoiceSetting = GetInvoiceSetting(invoiceSettingId);
            if (invoiceSetting != null)
                return invoiceSetting.Name;
            return null;
        }
     
        public IEnumerable<InvoiceSettingInfo> GetInvoiceSettingsInfo(InvoiceSettingFilter filter)
        {
            var invoiceSettings = GetCachedInvoiceSettings();
            if (filter != null)
            {
            }
            return invoiceSettings.MapRecords(InvoiceSettingInfoMapper);
        }
        public IEnumerable<InvoiceSetting> GetInvoiceSettings(Guid invoiceTypeId)
        {
            return GetCachedInvoiceSettings().FindAllRecords(x=>x.InvoiceTypeId == invoiceTypeId);
        }
        public InvoiceSetting GetDefaultInvoiceSetting(Guid invoiceTypeId)
        {
            var invoiceSettings = GetInvoiceSettings(invoiceTypeId);
            foreach(var item in invoiceSettings)
            {
                if (item.IsDefault)
                    return item;
            }
            throw new NullReferenceException(string.Format("No default setting available for invoicetypeid {0}", invoiceTypeId));
        }
        public Vanrise.Entities.UpdateOperationOutput<InvoiceSettingDetail> SetInvoiceSettingDefault(Guid invoiceSettingId)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<InvoiceSettingDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();

            if (dataManager.SetInvoiceSettingDefault(invoiceSettingId))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.UpdatedObject = InvoiceSettingDetailMapper(GetInvoiceSetting(invoiceSettingId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;

        }
        public T GetInvoiceSettingDetailByType<T>(Guid invoiceSettingId) where T : InvoiceSettingPart
        {
            var invoiceSetting = GetInvoiceSetting(invoiceSettingId);
            var configClass = Activator.CreateInstance(typeof(T)) as InvoiceSettingPart;
            InvoiceSettingPart invoiceSettingPart;
            if (invoiceSetting.Details.InvoiceSettingParts.TryGetValue(configClass.ConfigId, out invoiceSettingPart))
                return invoiceSettingPart as T;
            else
                return null;
        }
        public List<InvoiceSettingPartUISection> GetOverridableInvoiceSetting(Guid invoiceSettingId)
        {
            var invoiceSetting = GetInvoiceSetting(invoiceSettingId);
            if(invoiceSetting == null)
                throw new NullReferenceException("invoiceSetting");

            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceSetting.InvoiceTypeId);
            if (invoiceType == null)
                throw new NullReferenceException("invoiceType");
            if (invoiceType.Settings == null)
                throw new NullReferenceException("invoiceType.Settings");

            List<InvoiceSettingPartUISection> invoiceSettingPartUISections = null;
            if (invoiceType.Settings.InvoiceSettingPartUISections != null)
            {
                invoiceSettingPartUISections = new List<InvoiceSettingPartUISection>();
                foreach(var section in invoiceType.Settings.InvoiceSettingPartUISections)
                {
                    InvoiceSettingPartUISection invoiceSettingPartUISection = new InvoiceSettingPartUISection
                    {
                        SectionTitle = section.SectionTitle
                    };
                    if(section.Rows != null)
                    {
                        invoiceSettingPartUISection.Rows = new List<InvoiceSettingPartUIRow>();
                        foreach(var row in section.Rows)
                        {
                            InvoiceSettingPartUIRow invoiceSettingPartUIRow = new InvoiceSettingPartUIRow();
                            if(row.Parts != null)
                            {
                                invoiceSettingPartUIRow.Parts = new List<InvoiceSettingPartDefinition>();
                                foreach(var part in row.Parts)
                                {
                                    if (part.IsOverridable)
                                    {
                                        InvoiceSettingPartDefinition invoiceSettingPartDefinition = new InvoiceSettingPartDefinition
                                        {
                                            IsOverridable = part.IsOverridable,
                                            PartDefinitionSetting = part.PartDefinitionSetting,
                                            PartConfigId = part.PartConfigId
                                        };
                                        invoiceSettingPartUIRow.Parts.Add(invoiceSettingPartDefinition);
                                    }
                                }
                                if (invoiceSettingPartUIRow.Parts != null && invoiceSettingPartUIRow.Parts.Count >0 )
                                {
                                    invoiceSettingPartUISection.Rows.Add(invoiceSettingPartUIRow);
                                }
                            }
                        }
                    }
                    if(invoiceSettingPartUISection.Rows != null && invoiceSettingPartUISection.Rows.Count > 0)
                    {
                        invoiceSettingPartUISections.Add(invoiceSettingPartUISection);
                    }
                }
            }
            return invoiceSettingPartUISections;
        }

        public AutomaticInvoiceSettingPartRuntime GetAutomaticInvoiceSettingPartRuntime(Guid invoiceTypeId)
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            return new AutomaticInvoiceSettingPartRuntime
            {
                AutomaticInvoiceActions = invoiceType.Settings.AutomaticInvoiceActions,
                InvoiceAttachments = invoiceType.Settings.InvoiceAttachments
            };
        }
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IInvoiceSettingDataManager _dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreInvoiceSettingsUpdated(ref _updateHandle);
            }
        }


        private class InvoiceSettingLoggableEntity : VRLoggableEntityBase
        {

            static InvoiceSettingManager s_invoiceTypeManager = new InvoiceSettingManager();
            static InvoiceTypeManager _invoiceTypeManager = new InvoiceTypeManager();
            Guid _invoiceTypeId;


            public InvoiceSettingLoggableEntity(Guid invoiceTypeId)
            {
                _invoiceTypeId = invoiceTypeId;
            }

            public override string EntityUniqueName
            {
                get { return String.Format("VR_Invoice_InvoiceSetting_{0}", _invoiceTypeId); }
            }

            public override string EntityDisplayName
            {
                get { return String.Format(_invoiceTypeManager.GetInvoiceTypeName(_invoiceTypeId), "_InvoiceSettings"); }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Invoice_InvoiceSetting_ViewHistoryItem"; }
            }


            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                InvoiceSetting invoiceSetting = context.Object.CastWithValidate<InvoiceSetting>("context.Object");
                return invoiceSetting.InvoiceSettingId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                InvoiceSetting invoiceSetting = context.Object.CastWithValidate<InvoiceSetting>("context.Object");
                return s_invoiceTypeManager.GetInvoiceSettingName(invoiceSetting.InvoiceSettingId);
            }

            public override string ModuleName
            {
                get { return "Invoice"; }
            }
        }
        #endregion

        #region Private Methods
        private Dictionary<Guid, InvoiceSetting> GetCachedInvoiceSettings()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetInvoiceSettings",
              () =>
              {
                  IInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();
                  IEnumerable<InvoiceSetting> invoiceSettings = dataManager.GetInvoiceSettings();
                  return invoiceSettings.ToDictionary(c => c.InvoiceSettingId, c => c);
              });
        }
        private bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        #endregion

        #region Mappers
        private InvoiceSettingDetail InvoiceSettingDetailMapper(InvoiceSetting invoiceSettingObject)
        {
            InvoiceSettingDetail invoiceSettingDetail = new InvoiceSettingDetail();
            invoiceSettingDetail.Entity = invoiceSettingObject;
            return invoiceSettingDetail;
        }
        private InvoiceSettingInfo InvoiceSettingInfoMapper(InvoiceSetting invoiceSettingObject)
        {
            return new InvoiceSettingInfo
            {
                InvoiceSettingId = invoiceSettingObject.InvoiceSettingId,
                Name = invoiceSettingObject.Name
            };
        }
        #endregion

        
    }
}
