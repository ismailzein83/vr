using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class ZooManager
    {
        #region Public Methods

        public IDataRetrievalResult<ZooDetail> GetFilteredZoos(DataRetrievalInput<ZooQuery> input)
        {
            var allZoos = GetAllZoos();
            Func<Zoo, bool> filterExpression = (zoo) =>
            {
                if (input.Query.Name != null && !input.Query.Name.ToLower().Contains(zoo.Name.ToLower()))
                    return false;

                if (input.Query.Sizes != null && !input.Query.Sizes.Contains(zoo.Size))
                    return false;

                return true;
            };

            return DataRetrievalManager.Instance.ProcessResult(input, allZoos.ToBigResult(input, filterExpression, ZooDetailMapper));
        }

        public Zoo GetZooById(long zooId)
        {
            var allZoos = GetAllZoos();
            return allZoos.GetRecord(zooId);
        }

        public InsertOperationOutput<ZooDetail> AddZoo(Zoo zoo)
        {
            InsertOperationOutput<ZooDetail> insertOperationOutput = new InsertOperationOutput<ZooDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long zooId = -1;

            IZooDataManager zooDataManager = DemoModuleFactory.GetDataManager<IZooDataManager>();
            bool insertActionSuccess = zooDataManager.Insert(zoo, out zooId);
            if (insertActionSuccess)
            {
                zoo.ZooId = zooId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = this.ZooDetailMapper(zoo);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<ZooDetail> UpdateZoo(Zoo zoo)
        {
            UpdateOperationOutput<ZooDetail> updateOperationOutput = new UpdateOperationOutput<ZooDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IZooDataManager zooDataManager = DemoModuleFactory.GetDataManager<IZooDataManager>();
            bool updateActionSuccess = zooDataManager.Update(zoo);
            if (updateActionSuccess)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = this.ZooDetailMapper(zoo);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        private Dictionary<long, Zoo> GetAllZoos()
        {
            IZooDataManager zooDataManager = DemoModuleFactory.GetDataManager<IZooDataManager>();
            List<Zoo> allZoos = zooDataManager.GetZoos();

            return allZoos.ToDictionary(zoo => zoo.ZooId, zoo => zoo);
        }

        #endregion

        #region Mappers

        private ZooDetail ZooDetailMapper(Zoo zoo)
        {
            return new ZooDetail()
            {
                ZooId = zoo.ZooId,
                Name = zoo.Name,
                City = zoo.City,
                Size = zoo.Size
            };
        }

        #endregion
    }
}
