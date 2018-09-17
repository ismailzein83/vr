using System;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.BusinessObjectDataStore
{
    public class DataSourceBusinessObjectDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("c5e47d7e-924e-4305-923a-8ca69b7f275b"); } }

        public override bool DoesSupportFilterOnAllFields { get { return false; } }

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            //DateTime loadRecordsFromTime = DateTime.Now.AddHours(-1);

            //var manager = new DataSourceImportedBatchManager();

            //var dataRecords = manager.GetDataSourceBusinessObject(loadRecordsFromTime);

            //foreach (var dataRecord in dataRecords)
            //{
            //    context.OnRecordLoaded(DataRecordObjectMapper(dataRecord), DateTime.Now);
            //}
        }

        //private DataRecordObject DataRecordObjectMapper(DataSourceProgressDataRecordType dataSourceProgress)
        //{
        //    var dataSourceProgressObject = new Dictionary<string, object>{
        //        {"DataSourceId", dataSourceProgress.DataSourceId},
        //        {"LastImportedBatchTime", dataSourceProgress.LastImportedBatchTime},
        //        {"NbImportedBatch",dataSourceProgress.NbImportedBatch},
        //        {"TotalRecordCount",dataSourceProgress.TotalRecordCount},
        //        {"MaxRecordCount",dataSourceProgress.MaxRecordCount},
        //        {"MinRecordCount",dataSourceProgress.MinRecordCount},
        //        {"MaxBatchSize",dataSourceProgress.MaxBatchSize},
        //        {"MinBatchSize",dataSourceProgress.MinBatchSize},
        //        {"NbInvalidBatch",dataSourceProgress.NbInvalidBatch},
        //        {"NbEmptyBatch",dataSourceProgress.NbEmptyBatch}
        //    };

        //    return new DataRecordObject(new Guid(""), dataSourceProgressObject);
        //}
    }
}
