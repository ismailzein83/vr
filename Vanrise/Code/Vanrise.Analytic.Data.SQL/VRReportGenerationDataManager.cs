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
    class VRReportGenerationDataManager : BaseSQLDataManager, IVRReportGenerationDataManager
    {
        #region ctor/Local Variables
        public VRReportGenerationDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion


        #region Public Methods

        public List<VRReportGeneration> GetVRReportGenerations()
        {
            return GetItemsSP("Analytic.sp_VRReportGeneration_GetAll", VRReportGenerationMapper);
        }

        public bool AreVRReportGenerationUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Analytic.VRReportGeneration", ref updateHandle);
        }

        public bool Insert(VRReportGeneration vRReportGenerationItem, out long reportId)
        {
            object id;
            string serializedSettings = vRReportGenerationItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vRReportGenerationItem.Settings) : null;
            int nbOfRecordsAffected = ExecuteNonQuerySP("Analytic.sp_VRReportGeneration_Insert", out id, vRReportGenerationItem.Name, vRReportGenerationItem.Description, serializedSettings, vRReportGenerationItem.AccessLevel, vRReportGenerationItem.CreatedBy);
           
            bool result = (nbOfRecordsAffected > 0);
            if (result)
                reportId = (long)id;
            else
                reportId = 0;
            return result;
        }

        public bool Update(VRReportGeneration vRReportGenerationItem)
        {
            string serializedSettings = vRReportGenerationItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vRReportGenerationItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Analytic.sp_VRReportGeneration_Update", vRReportGenerationItem.ReportId, vRReportGenerationItem.Name, vRReportGenerationItem.Description,vRReportGenerationItem.AccessLevel, vRReportGenerationItem.LastModifiedBy, serializedSettings);
            return (affectedRecords > 0);
        }
        #endregion

        #region Mappers

        VRReportGeneration VRReportGenerationMapper(IDataReader reader)
        {
            return new VRReportGeneration
            {
                ReportId = (long)reader["ID"],
                Name = reader["Name"] as string,
                Description=reader["Description"]as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRReportGenerationSettings>(reader["Settings"] as string),
                AccessLevel = (AccessLevel)reader["AccessLevel"],
                CreatedBy = GetReaderValue<int>(reader, "CreatedBy"),
                CreatedTime = (DateTime)reader["CreatedTime"],
                LastModifiedTime = (DateTime)reader["LastModifiedTime"],
                LastModifiedBy = GetReaderValue<int>(reader, "LastModifiedBy")


            };
        }

        #endregion
    }
}
