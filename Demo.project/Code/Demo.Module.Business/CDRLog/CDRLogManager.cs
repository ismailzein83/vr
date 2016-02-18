using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Data;
using Demo.Module.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;
using System.ComponentModel;
namespace Demo.Module.Business
{
    public class CDRLogManager
    {
       public Vanrise.Entities.IDataRetrievalResult<CDRLogDetail> GetCDRLogData(Vanrise.Entities.DataRetrievalInput<CDRQuery> input)
        {
            ICDRLogDataManager datamanager = DemoModuleDataManagerFactory.GetDataManager<ICDRLogDataManager>();
            var cdrLogResult = datamanager.GetCDRLogData(input);
            BigResult<CDRLogDetail> cDRLogBigResultDetailMapper = new BigResult<CDRLogDetail>
            {
                Data = cdrLogResult.Data.MapRecords(CDRLogDetailMapper),
                ResultKey = cdrLogResult.ResultKey,
                TotalCount = cdrLogResult.TotalCount
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cDRLogBigResultDetailMapper);
        }
       public CDRLogDetail CDRLogDetailMapper(CDRLog cDRLog)
        {
            string dataSourceName = "";
            DataSourceDetail dataSource = new DataSourceManager().GetDataSource(cDRLog.DataSourceId);
            if (dataSource != null)
                dataSourceName = dataSource.Name;

            ServiceType serviceType = new ServiceTypeManager().GetServiceType(cDRLog.ServiceTypeId);
            string serviceTypeDescription = "";
            if (serviceType != null)
                serviceTypeDescription = serviceType.Description;

            CDRLogDetail cDRLogDetail = new CDRLogDetail
            {
                Entity = cDRLog,
                DataSourceName = dataSourceName,
                ServiceTypeDescription = serviceTypeDescription

            };

            var directionAttribute = Utilities.GetEnumAttribute<Direction, DescriptionAttribute>(cDRLog.DirectionType);
            if (directionAttribute != null)
                cDRLogDetail.DirectionDescription = directionAttribute.Description;

            var cdrType = Utilities.GetEnumAttribute<Demo.Module.Entities.Type, DescriptionAttribute>(cDRLog.CDRType);
            if (cdrType != null)
                cDRLogDetail.CDRTypeDescription = cdrType.Description;

            return cDRLogDetail;
        }
    }
}
