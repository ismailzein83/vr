using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneServiceManager
    {
        #region Public Methods

        public List<SupplierZoneService> GetSupplierZonesServicesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.GetSupplierZonesServicesEffectiveAfter(supplierId, minimumDate);
        }

        public List<SupplierDefaultService> GetSupplierDefaultServicesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.GetSupplierDefaultServicesEffectiveAfter(supplierId, minimumDate);
        }

        private IEnumerable<SupplierZoneService> GetSupplierZonesServicesEffectiveAfterByZoneName(string ZoneName, int supplierId, DateTime effectiveDate, out SupplierZone effectiveSupplierZone)
        {
            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            SupplierZoneManager zoneManager = new SupplierZoneManager();
            IEnumerable<long> zoneIds = zoneManager.GetSupplierZoneIdsEffectiveByZoneName(ZoneName, effectiveDate, supplierId);
            string strZoneIds = null;
            if (zoneIds != null && zoneIds.Count() > 0)
            {
                strZoneIds = string.Join(",", zoneIds);
            }
            IEnumerable<SupplierZoneService> supplierZoneServiceList = dataManager.GetSupplierZonesServicesEffectiveAfterByZoneIds(supplierId, effectiveDate, strZoneIds);
            effectiveSupplierZone = zoneManager.GetEffectiveSupplierZoneByZoneIds(zoneIds, effectiveDate);
            return supplierZoneServiceList;
        }

        public Vanrise.Entities.UpdateOperationOutput<SupplierEntityServiceDetail> UpdateSupplierZoneService(SupplierZoneServiceToEdit serviceObject)
        {
            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();

            SupplierZone effectiveSupplierZone;

            IEnumerable<SupplierZoneService> listSupplierZoneService = GetSupplierZonesServicesEffectiveAfterByZoneName(serviceObject.ZoneName, serviceObject.SupplierId,
                serviceObject.BED, out effectiveSupplierZone);

            Vanrise.Entities.UpdateOperationOutput<SupplierEntityServiceDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SupplierEntityServiceDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (effectiveSupplierZone == null)
            {
                updateOperationOutput.Message = String.Format("Zone {0} is not effective at date {1}", serviceObject.ZoneName, serviceObject.BED);
                return updateOperationOutput;
            }

            SupplierZoneManager zoneManager = new SupplierZoneManager();
            List<SupplierZoneServiceToClose> listOfZoneServiceToClose = new List<SupplierZoneServiceToClose>();
            SupplierZoneServiceToClose supplierZoneServiceToClose;

            DateTime closeDate;
            foreach (SupplierZoneService supplierZoneService in listSupplierZoneService)
            {
                closeDate = Utilities.Max(serviceObject.BED, supplierZoneService.BED);

                supplierZoneServiceToClose = new SupplierZoneServiceToClose()
                {
                    SupplierZoneServiceId = supplierZoneService.SupplierZoneServiceId,
                    CloseDate = closeDate
                };

                listOfZoneServiceToClose.Add(supplierZoneServiceToClose);
            }

            bool updateActionSucc = dataManager.Update(listOfZoneServiceToClose, effectiveSupplierZone.SupplierZoneId, this.ReserveIDRange(1), serviceObject);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;

                updateOperationOutput.UpdatedObject = new SupplierEntityServiceDetail
                {
                    SupplierZoneServiceId = serviceObject.SupplierZoneServiceId,
                    SupplierZoneId = serviceObject.SupplierZoneId,
                    Source = SupplierEntityServiceSource.SupplierZone,
                    BED = serviceObject.BED,
                    EED = null,
                    ZoneName = serviceObject.ZoneName,
                    Services = serviceObject.Services.Select(x => x.ServiceId).ToList()
                };
            }
            return updateOperationOutput;
        }
        public Vanrise.Entities.IDataRetrievalResult<SupplierEntityServiceDetail> GetFilteredSupplierZoneServices(Vanrise.Entities.DataRetrievalInput<SupplierZoneServiceQuery> input)
        {
            VRActionLogger.Current.LogGetFilteredAction(SupplierZoneServiceLoggableEntity.Instance, input);
            return BigDataManager.Instance.RetrieveData(input, new SupplierZoneServiceRequestHandler());
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSupplierZoneServiceType(), numberOfIDs, out startingId);
            return startingId;
        }

        public bool Insert(int supplierId, List<ZoneService> services)
        {
            SupplierDefaultService supplierDefaultService = this.PrepareNewDefaultService(supplierId, services);
            supplierDefaultService.SupplierZoneServiceId = this.ReserveIDRange(1);

            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.Insert(supplierDefaultService);
        }

        public bool InsertSupplierDefaultService(SupplierDefaultService supplierDefaultService)
        {
            supplierDefaultService.SupplierZoneServiceId = this.ReserveIDRange(1);

            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.Insert(supplierDefaultService);
        }

        public void UpdateSupplierDefaultService(int supplierId, List<ZoneService> services)
        {
            SupplierDefaultService supplierZoneService = this.GetSupplierDefaultServiceBySupplier(supplierId, DateTime.Today);

            if (supplierZoneService == null)
            {
                this.Insert(supplierId, services);
            }
            else if (!this.HasSameServices(supplierZoneService.ReceivedServices, services))
            {
                SupplierDefaultService newSupplierZoneService = this.PrepareNewDefaultService(supplierId, services);
                this.CloseOverlappedDefaultService(supplierZoneService.SupplierZoneServiceId, newSupplierZoneService, DateTime.Today);
            }

            //TODO: MJA check how to return boolean from this method to indicate success
            //This boolean should be used in carrier account manager to reflect the correct update status
        }

        public int GetSupplierZoneServiceTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierZoneServiceType());
        }

        public Type GetSupplierZoneServiceType()
        {
            return this.GetType();
        }

        #endregion

        #region Private Methods
        private SupplierEntityServiceDetail SupplierEntityServiceDetailMapper(SupplierEntityService supplierEntityService)
        {
            SupplierEntityServiceDetail detail = new SupplierEntityServiceDetail()
            {
                Source = supplierEntityService.Source,
                BED = supplierEntityService.BED,
                EED = null,
                Services = supplierEntityService.Services.Select(x => x.ServiceId).ToList()
            };

            return detail;
        }

        private class SupplierZoneServiceRequestHandler : BigDataRequestHandler<SupplierZoneServiceQuery, SupplierEntityServiceDetail, SupplierEntityServiceDetail>
        {
            public override SupplierEntityServiceDetail EntityDetailMapper(SupplierEntityServiceDetail entity)
            {
                return entity;
            }

            public override IEnumerable<SupplierEntityServiceDetail> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SupplierZoneServiceQuery> input)
            {
                SupplierZoneManager zoneManager = new SupplierZoneManager();
                IEnumerable<SupplierZone> supplierZones = zoneManager.GetSupplierZones(input.Query.SupplierId, input.Query.EffectiveOn);

                if (input.Query.ZoneIds != null)
                    supplierZones = supplierZones.FindAllRecords(item => input.Query.ZoneIds.Contains(item.SupplierZoneId));

                SupplierZoneServiceLocator zoneServiceLocator = new SupplierZoneServiceLocator(new SupplierZoneServiceReadAllWithCache(input.Query.EffectiveOn));

                List<SupplierEntityServiceDetail> supplierEntityServicesDetail = new List<SupplierEntityServiceDetail>();

                if (input.Query.ServiceIds != null)
                    supplierZones = GetSupplierZoneServicesByServiceId(input.Query.ServiceIds, supplierZones, zoneServiceLocator, input.Query.EffectiveOn);

                foreach (SupplierZone supplierZone in supplierZones)
                {
                    var entity = zoneServiceLocator.GetSupplierZoneServices(supplierZone.SupplierId, supplierZone.SupplierZoneId, input.Query.EffectiveOn);

                    supplierEntityServicesDetail.Add(new SupplierEntityServiceDetail
                    {
                        SupplierZoneServiceId = entity.SupplierZoneServiceId,
                        Source = entity.Source,
                        BED = entity.BED,
                        EED = entity.EED,
                        ZoneName = supplierZone.Name,
                        SupplierZoneId = supplierZone.SupplierZoneId,
                        Services = entity.Services.Select(x => x.ServiceId).ToList()
                    });
                }

                return supplierEntityServicesDetail;
            }

            protected override ResultProcessingHandler<SupplierEntityServiceDetail> GetResultProcessingHandler(DataRetrievalInput<SupplierZoneServiceQuery> input, BigResult<SupplierEntityServiceDetail> bigResult)
            {
                return new ResultProcessingHandler<SupplierEntityServiceDetail>
                {
                    ExportExcelHandler = new SupplierEntityServiceExcelExportHandler()
                };
            }
        }

        private class SupplierEntityServiceExcelExportHandler : ExcelExportHandler<SupplierEntityServiceDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierEntityServiceDetail> context)
            {
                ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();

                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier Zone Services",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.ZoneName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Services == null ? "" : zoneServiceConfigManager.GetZoneServicesNames(record.Services) });
                            row.Cells.Add(new ExportExcelCell { Value = record.BED });
                            row.Cells.Add(new ExportExcelCell { Value = record.EED });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private static List<SupplierZone> GetSupplierZoneServicesByServiceId(IEnumerable<int> servicesIds, IEnumerable<SupplierZone> supplierZones,
            SupplierZoneServiceLocator zoneServiceLocator, DateTime effectiveOn)
        {
            List<SupplierZone> supplierZoneFilterdByServiceId = new List<SupplierZone>();

            foreach (SupplierZone supplierZone in supplierZones)
            {
                var entity = zoneServiceLocator.GetSupplierZoneServices(supplierZone.SupplierId, supplierZone.SupplierZoneId, effectiveOn);
                var services = entity.Services.Select(x => x.ServiceId);
                foreach (var serviceId in servicesIds)
                {
                    if (services.Contains(serviceId))
                    {
                        supplierZoneFilterdByServiceId.Add(supplierZone);
                        break;
                    }
                }
            }

            return supplierZoneFilterdByServiceId;
        }

        private SupplierDefaultService GetSupplierDefaultServiceBySupplier(int supplierId, DateTime effectiveOn)
        {
            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.GetSupplierDefaultServiceBySupplier(supplierId, effectiveOn);
        }

        private bool HasSameServices(List<ZoneService> receivedServices, List<ZoneService> defaultServices)
        {
            if (receivedServices.Count != defaultServices.Count)
                return false;
            foreach (ZoneService zoneService in receivedServices)
            {
                if (!defaultServices.Any(item => item.ServiceId == zoneService.ServiceId))
                    return false;
            }

            return true;
        }

        private bool CloseOverlappedDefaultService(long supplierZoneServiceId, SupplierDefaultService supplierDefaultService, DateTime effectiveDate)
        {
            supplierDefaultService.SupplierZoneServiceId = this.ReserveIDRange(1);

            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.CloseOverlappedDefaultService(supplierZoneServiceId, supplierDefaultService, effectiveDate);
        }

        private SupplierDefaultService PrepareNewDefaultService(int supplierId, List<ZoneService> services)
        {
            return new SupplierDefaultService()
            {
                EffectiveServices = services,
                ReceivedServices = services,
                BED = DateTime.Today,
                SupplierId = supplierId,
                EED = null
            };
        }

        #endregion

        private class SupplierZoneServiceLoggableEntity : VRLoggableEntityBase
        {
            public static SupplierZoneServiceLoggableEntity Instance = new SupplierZoneServiceLoggableEntity();

            private SupplierZoneServiceLoggableEntity()
            {

            }

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SupplierZoneService"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Supplier Zone Service"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SupplierZoneService_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SupplierZoneService supplierZoneSrvice = context.Object.CastWithValidate<SupplierZoneService>("context.Object");
                return supplierZoneSrvice.SupplierZoneServiceId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                return null;
            }
        }
    }
}
