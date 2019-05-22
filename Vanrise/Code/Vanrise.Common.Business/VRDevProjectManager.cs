//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Entities;
//using Vanrise.Common.Data;
//using Vanrise.Caching;
//using Vanrise.Security.Entities;
//using Vanrise.Common;
//using Vanrise.GenericData.Entities;

//namespace Vanrise.Common.Business
//{
//    public class VRDevProjectManager
//    {
//        static Guid businessEntityDefinitionId = new Guid("6954527C-6DE0-411C-859D-E044165D58AF");

//        #region Public Methods
//        public IEnumerable<VRDevProjectInfo> GetVRDevProjectsInfo(VRDevProjectInfoFilter filter = null)
//        {
//            var vrDevProjects = GetCachedVRDevProjects();
//            if (vrDevProjects != null && vrDevProjects.Count > 0)
//            {
//                return vrDevProjects.MapRecords(VRDevProjectInfoMapper).OrderBy(vrDevProject => vrDevProject.Name);
//            }
//            return null;
//        }
//        public string GetVRDevProjectName(Guid vrDevProjectId)
//        {
//            return GetCachedVRDevProjects().GetRecord(vrDevProjectId).Name;
//        }
//        #endregion

//        #region Private Classes
//        #endregion

//        #region Private Methods

//        private Dictionary<Guid, VRDevProject> GetCachedVRDevProjects()
//        {
//            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
//            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedVRDevProjects", businessEntityDefinitionId, () =>
//            {
//                Dictionary<Guid, VRDevProject> result = new Dictionary<Guid, VRDevProject>();
//                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
//                if (genericBusinessEntities != null)
//                {
//                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
//                    {
//                        VRDevProject vrDevProject = new VRDevProject()
//                        {
//                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
//                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
//                        };

//                        var createdTime = genericBusinessEntity.FieldValues.GetRecord("CreatedTime");
//                        if (createdTime != null)
//                            vrDevProject.CreatedTime = (DateTime)createdTime;

//                        var lastModifiedTime = genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime");
//                        if (createdTime != null)
//                            vrDevProject.LastModifiedTime = (DateTime)lastModifiedTime;

//                        result.Add(vrDevProject.ID, vrDevProject);
//                    }
//                }
//                return result;
//            });
//        }

//        #endregion

//        #region Mappers
//        public VRDevProjectInfo VRDevProjectInfoMapper(VRDevProject vrDevProject)
//        {
//            return new VRDevProjectInfo()
//            {
//                ID = vrDevProject.ID,
//                Name = vrDevProject.Name
//            };
//        }

//        #endregion
//    }

  
//}
