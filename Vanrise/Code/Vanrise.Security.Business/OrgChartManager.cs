using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class OrgChartManager
    {
        public List<OrgChart> GetOrgCharts()
        {
            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            return dataManager.GetOrgCharts();
        }

        public List<OrgChart> GetFilteredOrgCharts(int fromRow, int toRow, string name)
        {
            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            return dataManager.GetFilteredOrgCharts(fromRow, toRow, name);
        }

        public OrgChart GetOrgChartById(int orgChartId)
        {
            IOrgChartDataManager datamanager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            return datamanager.GetOrgChartById(orgChartId);
        }

        public Vanrise.Entities.InsertOperationOutput<OrgChart> AddOrgChart(OrgChart orgChartObject)
        {
            InsertOperationOutput<OrgChart> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OrgChart>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int orgChartId = -1;

            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            bool insertActionSucc = dataManager.AddOrgChart(orgChartObject, out orgChartId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                orgChartObject.Id = orgChartId;
                insertOperationOutput.InsertedObject = orgChartObject;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<OrgChart> UpdateOrgChart(OrgChart orgChartObject)
        {
            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            bool updateActionSucc = dataManager.UpdateOrgChart(orgChartObject);

            UpdateOperationOutput<OrgChart> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<OrgChart>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = orgChartObject;
            }
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteOrgChart(int orgChartId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();

            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            
            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            bool updateActionSucc = dataManager.DeleteOrgChart(orgChartId);

            if (updateActionSucc)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }
    }
}
