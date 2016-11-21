using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.NumberingPlan.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class ZonePreviewManager
    {

        public Vanrise.Entities.IDataRetrievalResult<ZonePreview> GetFilteredZonePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ZonePreviewRequestHandler());
        }


        #region Private Classes

        private class ZonePreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, ZonePreview, ZonePreview>
        {
            public override ZonePreview EntityDetailMapper(ZonePreview entity)
            {
                ZonePreviewManager manager = new ZonePreviewManager();
                return manager.ZonePreviewDetailMapper(entity);
            }

            public override IEnumerable<ZonePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISaleZonePreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleZonePreviewDataManager>();
                return dataManager.GetFilteredZonePreview(input.Query);
            }
        }

        #endregion


        #region Private Mappers

        private ZonePreview ZonePreviewDetailMapper(ZonePreview zonePreview)
        {
            return zonePreview;
        }

        #endregion


    }
}
