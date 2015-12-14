using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
namespace TOne.WhS.Analytics.Business
{
    public class RawCDRManager
    {
        private readonly SwitchManager _switchManager;
        public RawCDRManager()
        {
            _switchManager = new SwitchManager();
        }
        public Vanrise.Entities.IDataRetrievalResult<RawCDRLogDetail> GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRInput> input)
        {
            IRawCDRDataManager datamanager = AnalyticsDataManagerFactory.GetDataManager<IRawCDRDataManager>();
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
            Switch switchEntity = _switchManager.GetSwitch(rawCDRLog.SwitchID);

            RawCDRLogDetail rawCDRLogDetail = new RawCDRLogDetail
            {
                Entity = rawCDRLog,
                SwitchName = switchEntity != null ? switchEntity.Name : "N/A",
            };
            return rawCDRLogDetail;
        }
    }
}
