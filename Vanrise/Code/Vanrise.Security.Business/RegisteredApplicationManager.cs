using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.Security.Business
{
    public class RegisteredApplicationManager
    {

        static Guid beDefinitionId = new Guid("a7b4c14a-a11a-42ba-a85f-c0f030a99b94");


        public IEnumerable<RegisteredApplication> GetAllRegisteredApplications()
        {
            var cachedRegisteredApplications = this.GetCachedRegisteredApplications();
            if (cachedRegisteredApplications == null)
                return null;

            return cachedRegisteredApplications.Values;
        }

        public RegisteredApplication GetRegisteredApplicationbyId(Guid applicationId)
        {
            var cachedRegisteredApplications = this.GetCachedRegisteredApplications();
            if (cachedRegisteredApplications == null)
                return null;

            return cachedRegisteredApplications.GetRecord(applicationId);
        }

        public IEnumerable<RegisteredApplicationInfo> GetRegisteredApplicationsInfo(RegisteredApplicationFilter filter)
        {
            IEnumerable<RegisteredApplication> registeredApplications = GetAllRegisteredApplications();

            if (registeredApplications == null)
                return null;

            Func<RegisteredApplication, bool> filterPredicate = (registeredApplication) =>
            {
                if (filter != null && filter.Filters != null)
                {
                    foreach (var registeredApplicationFilter in filter.Filters)
                    {
                        RegisteredApplicationFilterContext context = new RegisteredApplicationFilterContext() { RegisteredApplication = registeredApplication };
                        if (registeredApplicationFilter.IsExcluded(context))
                            return false;
                    }
                }

                return true;
            };

            return registeredApplications.MapRecords(RegisteredApplicationInfoMapper, filterPredicate);
        }

        public IEnumerable<RegisteredApplicationInfo> GetRemoteRegisteredApplicationsInfo(Guid securityProviderId, string serializedFilter)
        {
            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(securityProviderId);
            securityProvider.ThrowIfNull("securityProvider", securityProviderId);

            SecurityProviderGetRemoteRegisteredApplicationsInfoContext context = new SecurityProviderGetRemoteRegisteredApplicationsInfoContext() { SerializedFilter = serializedFilter };
            return securityProvider.Settings.ExtendedSettings.GetRemoteRegisteredApplicationsInfo(context);
        }

        public RegisterApplicationOutput RegisterApplication(string applicationName, string applicationURL)
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            var genericBusinessEntityToAdd = new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = beDefinitionId,
                FieldValues = new Dictionary<string, object>()
            };
            genericBusinessEntityToAdd.FieldValues.Add("Name", applicationName);
            genericBusinessEntityToAdd.FieldValues.Add("URL", applicationURL);
            var insertOperationOutput = genericBusinessEntityManager.AddGenericBusinessEntity(genericBusinessEntityToAdd);
            if (insertOperationOutput.Result == Vanrise.Entities.InsertOperationResult.Succeeded)
            {
                var accplicationIdObject = insertOperationOutput.InsertedObject.FieldValues.GetRecord("ID");
                if (accplicationIdObject != null)
                {
                    var accplicationId = (Guid)accplicationIdObject.Value;
                    return new RegisterApplicationOutput() { ApplicationId = accplicationId };
                }
            }
            return null;
        }

        public bool HasRemoteApplications(int userId)
        {
            User user = new UserManager().GetUserbyId(userId);
            var result = GetRemoteRegisteredApplicationsInfo(user.SecurityProviderId, null);
            return result != null && result.Count() > 0;
        }

        private RegisteredApplicationInfo RegisteredApplicationInfoMapper(RegisteredApplication registeredApplication)
        {
            return new RegisteredApplicationInfo()
            {
                ApplicationId = registeredApplication.ApplicationId,
                Name = registeredApplication.Name,
                URL = registeredApplication.URL
            };
        }

        private Dictionary<Guid, RegisteredApplication> GetCachedRegisteredApplications()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedRegisteredApplications", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                Dictionary<Guid, RegisteredApplication> result = new Dictionary<Guid, RegisteredApplication>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        RegisteredApplication registeredApplication = new RegisteredApplication()
                        {
                            ApplicationId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string,
                            URL = genericBusinessEntity.FieldValues.GetRecord("URL") as string
                        };
                        result.Add(registeredApplication.ApplicationId, registeredApplication);
                    }
                }

                return result;
            });
        }

    }
}
