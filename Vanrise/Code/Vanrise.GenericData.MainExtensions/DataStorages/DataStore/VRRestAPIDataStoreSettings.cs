using System;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataStorages.DataRecordStorage;

namespace Vanrise.GenericData.MainExtensions.DataStorages.DataStore
{
    public class VRRestAPIDataStoreSettings : DataStoreSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("4829119D-F86F-4A6C-A6C0-CDB3FC8274C1"); }
        }

        public Guid ConnectionId { get; set; }


        public override void UpdateRecordStorage(IUpdateRecordStorageContext context)
        {
        }

        public override IDataRecordDataManager GetDataRecordDataManager(IGetRecordStorageDataManagerContext context)
        {
            return null;
        }

        public override ISummaryRecordDataManager GetSummaryDataRecordDataManager(IGetSummaryRecordStorageDataManagerContext context)
        {
            return null;
        }

        public override IRemoteRecordDataManager GetRemoteRecordDataManager(IGetRemoteRecordStorageDataManagerContext context)
        {
            return new VRRestAPIRecordDataManager(context.DataRecordStorage, this.ConnectionId);
        }

        public override bool IsRemoteDataStore
        {
            get
            {
                return true;
            }
        }
    }
}