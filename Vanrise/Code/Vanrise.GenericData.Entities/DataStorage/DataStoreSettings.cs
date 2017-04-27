using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataStoreSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual bool IsRemoteDataStore { get { return false; } }

        public abstract void UpdateRecordStorage(IUpdateRecordStorageContext context);

        public abstract IDataRecordDataManager GetDataRecordDataManager(IGetRecordStorageDataManagerContext context);

        public abstract ISummaryRecordDataManager GetSummaryDataRecordDataManager(IGetSummaryRecordStorageDataManagerContext context);

        public virtual IRemoteRecordDataManager GetRemoteRecordDataManager(IGetRemoteRecordStorageDataManagerContext context)
        {
            return null;
        }

        public abstract void CreateTempStorage(ICreateTempStorageContext context);

        public abstract void FillDataRecordStorageFromTempStorage(IFillDataRecordStorageFromTempStorageContext context);

        public abstract void DropStorage(IDropStorageContext context);

        public abstract int GetStorageRowCount(IGetStorageRowCountContext context);
    }
}
