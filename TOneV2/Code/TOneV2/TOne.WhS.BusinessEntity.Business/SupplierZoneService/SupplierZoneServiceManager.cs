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

        public Vanrise.Entities.IDataRetrievalResult<SupplierEntityServiceDetail> GetFilteredSupplierZoneServices(Vanrise.Entities.DataRetrievalInput<SupplierZoneServiceQuery> input)
        {
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
                Entity = supplierEntityService
               
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

                if(input.Query.ZoneIds != null)
                     supplierZones = supplierZones.FindAllRecords(item => input.Query.ZoneIds.Contains(item.SupplierZoneId));

                SupplierZoneServiceLocator zoneServiceLocator = new SupplierZoneServiceLocator(new SupplierZoneServiceReadAllWithCache(input.Query.EffectiveOn));
                
                List<SupplierEntityServiceDetail> supplierEntityServicesDetail = new List<SupplierEntityServiceDetail>();
               
                foreach (SupplierZone supplierZone in supplierZones)
                {
                    SupplierEntityServiceDetail supplierEntityServiceDetail = new SupplierEntityServiceDetail();
                    supplierEntityServiceDetail.Entity = zoneServiceLocator.GetSupplierZoneServices(supplierZone.SupplierId, supplierZone.SupplierZoneId, input.Query.EffectiveOn);
                     supplierEntityServiceDetail.ZoneName = supplierZone.Name;
                     supplierEntityServiceDetail.Services = supplierEntityServiceDetail.Entity.Services.Select(x => x.ServiceId).ToList();         
                    supplierEntityServicesDetail.Add(supplierEntityServiceDetail);
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
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.ZoneName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Services == null ? "" : zoneServiceConfigManager.GetZoneServicesNames(record.Services) });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.EED });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
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
                SupplierId = supplierId
            };
        }

        #endregion
    }
}
