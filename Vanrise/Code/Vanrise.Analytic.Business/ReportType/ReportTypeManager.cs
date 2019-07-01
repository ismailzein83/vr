using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public class ReportTypeManager
    {
        //static Guid businessEntityDefinitionId = new Guid("3a423ccc-742f-4ecd-a756-5a67bb763e5f");


        //#region Public Methods
        //#endregion


       // #region Private Methods
        //private Dictionary<Guid, VRScheduledReport> GetCachedReportTypes()
        //{
        //    IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
        //    return genericBusinessEntityManager.GetCachedOrCreate("GetCachedAccountManager", businessEntityDefinitionId, () =>
        //    {
        //        Dictionary<Guid, VRScheduledReport> result = new Dictionary<Guid, VRScheduledReport>();
        //        IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
        //        if (genericBusinessEntities != null)
        //        {
        //            foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
        //            {
        //                VRScheduledReport scheduledReport = new VRScheduledReport()
        //                {
        //                    VRScheduledReportId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
        //                    Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
        //                    Settings = Serializer.Deserialize<VRScheduledReportSettings>(genericBusinessEntity.FieldValues.GetRecord("Settings") as string),
        //                    CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
        //                    LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
        //                    CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
        //                    LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
        //                };
        //                result.Add(scheduledReport.VRScheduledReportId, scheduledReport);
        //            }
        //        }
        //        return result;
        //    });
        //}
        //#endregion


        //#region Mappers 
        //#endregion
    }
}
