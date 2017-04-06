using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Data;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.DataParser.Entities;
using Vanrise.Common;
namespace Vanrise.DataParser.Business
{
    public class ParserTypeManager
    { 
        #region Public Methods

        public IDataRetrievalResult<ParserTypeDetail> GetFilteredParserTypes(Vanrise.Entities.DataRetrievalInput<ParserTypeQuery> input)
        {
            var allParserTypes = GetCachedParserTypes();

            Func<ParserType, bool> filterExpression = (parserType) =>
                 (input.Query.Name == null || parserType.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allParserTypes.ToBigResult(input, filterExpression,ParserTypeDetailMapper));
        }

        public ParserType GetParserType(Guid parserTypeId)
        {
            var allParserTypes = GetCachedParserTypes();
            return allParserTypes.GetRecord(parserTypeId);
        }

        public Vanrise.Entities.InsertOperationOutput<ParserTypeDetail> AddParserType(ParserType parserType)
        {
            Vanrise.Entities.InsertOperationOutput<ParserTypeDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ParserTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;



            IParserTypeDataManager dataManager = DataParserDataManagerFactory.GetDataManager<IParserTypeDataManager>();
            parserType.ParserTypeId = Guid.NewGuid();
            bool insertActionSucc = dataManager.Insert(parserType);
            if (insertActionSucc)
            {
               
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ParserTypeDetailMapper(parserType);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<ParserTypeDetail> UpdateParserType(ParserType parserType)
        {
            IParserTypeDataManager dataManager = DataParserDataManagerFactory.GetDataManager<IParserTypeDataManager>();

            bool updateActionSucc = dataManager.Update(parserType);
            Vanrise.Entities.UpdateOperationOutput<ParserTypeDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ParserTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ParserTypeDetailMapper(parserType);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

#endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IParserTypeDataManager _dataManager = DataParserDataManagerFactory.GetDataManager<IParserTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreParserTypesUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods
        private Dictionary<Guid,ParserType> GetCachedParserTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetParserTypes",
              () =>
              {
                  IParserTypeDataManager dataManager = DataParserDataManagerFactory.GetDataManager<IParserTypeDataManager>();
                  IEnumerable<ParserType> parserTypes = dataManager.GetParserTypes();
                  return parserTypes.ToDictionary(p =>p.ParserTypeId, p => p);
              });
        }
        #endregion

        #region Mappers

        public ParserTypeDetail ParserTypeDetailMapper(ParserType parserType)
        {
            ParserTypeDetail parserTypeDetail = new ParserTypeDetail();
            parserTypeDetail.Entity = parserType;
            return parserTypeDetail;
        }


        #endregion
    }
}
