using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataStoreSettings
    {
        public abstract Guid ConfigId { get;}

        public abstract void UpdateRecordStorage(IUpdateRecordStorageContext context);

        public abstract IDataRecordDataManager GetDataRecordDataManager(IGetRecordStorageDataManagerContext context);

        public abstract ISummaryRecordDataManager GetSummaryDataRecordDataManager(IGetSummaryRecordStorageDataManagerContext context);

        public virtual IRemoteRecordDataManager GetRemoteRecordDataManager(IGetRemoteRecordStorageDataManagerContext context)
        {
            return null;
        }

        public virtual bool IsRemoteDataStore
        {
            get { return false; }
        }
    }
}
