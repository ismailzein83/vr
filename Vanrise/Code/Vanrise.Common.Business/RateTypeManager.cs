﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Data;
using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class RateTypeManager : IBusinessEntityManager
    {
        #region ctor/Local Variables
        #endregion

        #region Public Methods

        public IDataRetrievalResult<Vanrise.Entities.RateTypeDetail> GetFilteredRateTypes(DataRetrievalInput<Vanrise.Entities.RateTypeQuery> input)
        {
            var allRateTypes = GetCachedRateTypes();
            Func<Vanrise.Entities.RateType, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));

            RateTypeExcelExportHandler rateTypeExcel = new RateTypeExcelExportHandler(input.Query);
            ResultProcessingHandler<RateTypeDetail> handler = new ResultProcessingHandler<RateTypeDetail>()
            {
                ExportExcelHandler = rateTypeExcel
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRateTypes.ToBigResult(input, filterExpression, RateTypeDetailMapper), handler);

        }

        private class RateTypeExcelExportHandler : ExcelExportHandler<RateTypeDetail>
        {
            private RateTypeQuery _query;
            public RateTypeExcelExportHandler(RateTypeQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RateTypeDetail> context)
            {
                if (context.BigResult == null)
                    throw new ArgumentNullException("context.BigResult");
                if (context.BigResult.Data == null)
                    throw new ArgumentNullException("context.BigResult.Data");
                ExportExcelSheet sheet = new ExportExcelSheet();
                sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Id" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });

                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.Rows.Add(row);
                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.RateTypeId });
                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                }
                context.MainSheet = sheet;
            }
        }
        public IEnumerable<Vanrise.Entities.RateTypeInfo> GetAllRateTypes()
        {
            return this.GetCachedRateTypes().MapRecords(RateTypeInfoMapper).OrderBy(x => x.Name);
        }
        public Vanrise.Entities.RateType GetRateType(int rateTypeId)
        {
            var allRateTypes = GetCachedRateTypes();
            return allRateTypes.GetRecord(rateTypeId);
        }
        public string GetRateTypeName(int rateTypeId)
        {
            var rateType = GetRateType(rateTypeId);
            return (rateType != null) ? rateType.Name : null;
        }
        public Vanrise.Entities.InsertOperationOutput<Vanrise.Entities.RateTypeDetail> AddRateType(Vanrise.Entities.RateType rateType)
        {
            Vanrise.Entities.InsertOperationOutput<Vanrise.Entities.RateTypeDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<Vanrise.Entities.RateTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int rateTypeId = -1;

            IRateTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IRateTypeDataManager>();
            bool insertActionSucc = dataManager.Insert(rateType, out rateTypeId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                rateType.RateTypeId = rateTypeId;
                insertOperationOutput.InsertedObject = RateTypeDetailMapper(rateType);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Entities.RateTypeDetail> UpdateRateType(Vanrise.Entities.RateType rateType)
        {
            IRateTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IRateTypeDataManager>();

            bool updateActionSucc = dataManager.Update(rateType);
            Vanrise.Entities.UpdateOperationOutput<Vanrise.Entities.RateTypeDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<Vanrise.Entities.RateTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RateTypeDetailMapper(rateType);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods
        private class CacheManager : BaseCacheManager
        {
            IRateTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IRateTypeDataManager>();

            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreRateTypesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, Vanrise.Entities.RateType> GetCachedRateTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRateTypes",
               () =>
               {
                   IRateTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IRateTypeDataManager>();
                   IEnumerable<Vanrise.Entities.RateType> rateTypes = dataManager.GetRateTypes();
                   return rateTypes.ToDictionary(x => x.RateTypeId, x => x);
               });
        }
        #endregion

        #region  Mappers
        private Vanrise.Entities.RateTypeInfo RateTypeInfoMapper(Vanrise.Entities.RateType rateType)
        {
            return new Vanrise.Entities.RateTypeInfo
            {
                Name = rateType.Name,
                RateTypeId = rateType.RateTypeId
            };
        }
        private Vanrise.Entities.RateTypeDetail RateTypeDetailMapper(Vanrise.Entities.RateType rateType)
        {
            return new Vanrise.Entities.RateTypeDetail
            {
                Entity = rateType,
            };
        }
        #endregion

        #region IBusinessEntityManager

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetRateTypeName(Convert.ToInt32(context.EntityId));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var rateType = context.Entity as RateType;
            return rateType.RateTypeId;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetRateType(context.EntityId);
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllRateTypes().Select(itm => itm as dynamic).ToList();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
