using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class BusinessObjectDataStoreSettings : Vanrise.GenericData.Entities.DataStoreSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C9FA9112-6D68-490C-BCBF-BD41A912A865"); }
        }

        public override void UpdateRecordStorage(IUpdateRecordStorageContext context)
        {
            throw new NotImplementedException();
        }

        public override IDataRecordDataManager GetDataRecordDataManager(IGetRecordStorageDataManagerContext context)
        {
            return new BusinessObjectDataRecordDataManager(context.DataRecordStorage.Settings.CastWithValidate<BusinessObjectDataRecordStorageSettings>("context.DataRecordStorage.Settings", context.DataRecordStorage.DataRecordStorageId),
                context.DataRecordStorage);
        }

        public override ISummaryRecordDataManager GetSummaryDataRecordDataManager(IGetSummaryRecordStorageDataManagerContext context)
        {
            throw new NotImplementedException();
        }

        public override void CreateTempStorage(ICreateTempStorageContext context)
        {
            throw new NotImplementedException();
        }

        public override void FillDataRecordStorageFromTempStorage(IFillDataRecordStorageFromTempStorageContext context)
        {
            throw new NotImplementedException();
        }

        public override void DropStorage(IDropStorageContext context)
        {
            throw new NotImplementedException();
        }

        public override int GetStorageRowCount(IGetStorageRowCountContext context)
        {
            throw new NotImplementedException();
        }
    }
}
