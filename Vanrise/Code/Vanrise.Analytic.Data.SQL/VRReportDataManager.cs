using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Analytic.Data.SQL
{
//    class VRReportDataManager : BaseSQLDataManager, IVRReportDataManager
//    {
//        #region ctor/Local Variables
//        public VRReportDataManager()
//            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
//        {

//        }

//        #endregion


    //    #region Public Methods

    //    public List<DataAnalysisDefinition> GetDataAnalysisDefinitions()
    //    {
    //        return GetItemsSP("Analytic.sp_DataAnalysisDefinition_GetAll", DataAnalysisDefinitionMapper);
    //    }

    //    public bool AreDataAnalysisDefinitionUpdated(ref object updateHandle)
    //    {
    //        return base.IsDataUpdated("Analytic.DataAnalysisDefinition", ref updateHandle);
    //    }

    //    public bool Insert(DataAnalysisDefinition dataAnalysisDefinitionItem)
    //    {
    //        string serializedSettings = dataAnalysisDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(dataAnalysisDefinitionItem.Settings) : null;
    //        int affectedRecords = ExecuteNonQuerySP("Analytic.sp_DataAnalysisDefinition_Insert", dataAnalysisDefinitionItem.DataAnalysisDefinitionId, dataAnalysisDefinitionItem.Name, serializedSettings);

    //        if (affectedRecords > 0)
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    public bool Update(DataAnalysisDefinition dataAnalysisDefinitionItem)
    //    {
    //        string serializedSettings = dataAnalysisDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(dataAnalysisDefinitionItem.Settings) : null;
    //        int affectedRecords = ExecuteNonQuerySP("Analytic.sp_DataAnalysisDefinition_Update", dataAnalysisDefinitionItem.DataAnalysisDefinitionId, dataAnalysisDefinitionItem.Name, serializedSettings);
    //        return (affectedRecords > 0);
    //    }

    //    #endregion


    //    #region Mappers

    //    DataAnalysisDefinition DataAnalysisDefinitionMapper(IDataReader reader)
    //    {
    //        DataAnalysisDefinition dataAnalysisDefinition = new DataAnalysisDefinition
    //        {
    //            DataAnalysisDefinitionId = (Guid) reader["ID"],
    //            Name = reader["Name"] as string,
    //            Settings = Vanrise.Common.Serializer.Deserialize<DataAnalysisDefinitionSettings>(reader["Settings"] as string) 
    //        };
    //        return dataAnalysisDefinition;
    //    }

    //    #endregion
    //}
}
