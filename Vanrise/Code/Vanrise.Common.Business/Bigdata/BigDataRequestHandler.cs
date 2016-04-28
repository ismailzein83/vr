using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    public abstract class BigDataRequestHandler<T, Q, R>
    {
        public abstract R EntityDetailMapper(Q entity);

        public abstract IEnumerable<Q> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<T> input);

        public virtual Entities.IDataRetrievalResult<R> AllRecordsToDataResult(Vanrise.Entities.DataRetrievalInput<T> input, IEnumerable<Q> allRecords)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, AllRecordsToBigResult(input, allRecords));
        }

        protected virtual Entities.BigResult<R> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<T> input, IEnumerable<Q> allRecords)
        {
            return allRecords.ToBigResult(input, null, (entity) => this.EntityDetailMapper(entity));
        }
    }
}
