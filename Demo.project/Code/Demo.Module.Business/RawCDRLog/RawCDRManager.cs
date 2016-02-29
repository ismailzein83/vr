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
    public class RawCDRManager
    {
       public Vanrise.Entities.IDataRetrievalResult<RawCDRLogDetail> GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRQuery> input)
        {
            IRawCDRDataManager datamanager = DemoModuleDataManagerFactory.GetDataManager<IRawCDRDataManager>();
            var rawCDRLogResult = datamanager.GetRawCDRData(input);
            BigResult<RawCDRLogDetail> rawCDRLogBigResultDetailMapper = new BigResult<RawCDRLogDetail>
            {
                Data = rawCDRLogResult.Data.MapRecords(RawCDRLogDetailMapper),
                ResultKey = rawCDRLogResult.ResultKey,
                TotalCount = rawCDRLogResult.TotalCount
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, rawCDRLogBigResultDetailMapper);
        }
        public RawCDRLogDetail RawCDRLogDetailMapper(RawCDRLog rawCDRLog)
        {
            string dataSourceName = "";
            DataSourceDetail dataSource =  new DataSourceManager().GetDataSourceDetail(rawCDRLog.DataSourceId);
            if (dataSource != null)
                dataSourceName = dataSource.Name;

            ServiceType serviceType = new ServiceTypeManager().GetServiceType(rawCDRLog.ServiceTypeId);
            string serviceTypeDescription = "";
            if (serviceType != null)
                serviceTypeDescription = serviceType.Description;

            RawCDRLogDetail rawCDRLogDetail = new RawCDRLogDetail
            {
                Entity = rawCDRLog,
                DataSourceName = dataSourceName,
                ServiceTypeDescription = serviceTypeDescription

            };

            var directionAttribute = Utilities.GetEnumAttribute<CDRDirection, DescriptionAttribute>(rawCDRLog.DirectionType);
            if (directionAttribute != null)
                rawCDRLogDetail.DirectionDescription = directionAttribute.Description;

            var cdrType = Utilities.GetEnumAttribute<Demo.Module.Entities.CDRType, DescriptionAttribute>(rawCDRLog.CDRType);
            if (cdrType != null)
                rawCDRLogDetail.CDRTypeDescription = cdrType.Description;

            return rawCDRLogDetail;
        }
    }
}
