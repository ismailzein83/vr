using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Runtime.Tasks
{

    #region TicketInfo classes

    public class TicketInfo
    {
        // public long ID { get; set; }
        public int TicketType { get; set; }
        public string Title { get; set; }
        public int AssignedGroup { get; set; }
    }

    public class GetCaseTicketInfos
    {
        public List<TicketInfo> Tickets { get; set; }
    }

    #endregion

    #region TicketManager class
    
    #endregion

    public class RDBJoinCode

    {

        void SetJoin(Vanrise.GenericData.RDBDataStorage.RDBDataStorageAddJoinRDBExpressionContext context)
        {
            string tableToJoinTableAlias = context.GetTableToJoinTableAlias();

            var join = context.RDBJoinContext.JoinSelect(tableToJoinTableAlias);

            var joinSelect = join.SelectQuery();

            joinSelect.From(Vanrise.GenericData.RDBDataStorage.RecordStorageRDBAnalyticDataProviderTable.GetRDBTableNameByRecordStorageId(new Guid("cacaa170-8d60-45de-833e-489d2784ed05")), "port", 1, true);

            var inlineJoinContext = joinSelect.Join();

            inlineJoinContext.JoinOnEqualOtherTableColumn(Vanrise.GenericData.RDBDataStorage.RecordStorageRDBAnalyticDataProviderTable.GetRDBTableNameByRecordStorageId(new Guid("5ae2fe1d-4056-4a28-bb72-6780f4aba39a")), "vertical", "Id", "port", "OLTVertival");

            var selectColumns = joinSelect.SelectColumns();
            selectColumns.Columns("Id");
            selectColumns.Column("vertical", "OLT", "OLT");

            joinSelect.Where().EqualsCondition("port", "Status").Value(new Guid("d65f6900-7e1f-479d-be8c-c5ecbc45c7c5"));


            join.On().EqualsCondition(context.GetJoinTableAlias("Splitter"), "OLT").Column(tableToJoinTableAlias, "OLT");
        }
    }

    public class ServiceCatalogManager
    {
        static Guid s_businessEntityDefinitionId = new Guid("4d591e34-1ee9-44f4-9b32-98accc1a59b3");

        public ServiceCatalog GetServiceCatalog(Guid serviceId)
        {
            return GetCachedServiceCatalogs().GetRecord(serviceId);
        }

        public Dictionary<Guid, ServiceCatalog> GetCachedServiceCatalogs()
        {
            var genericBEManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<Vanrise.GenericData.Entities.IGenericBusinessEntityManager>();
            //return genericBEManager.GetCachedOrCreate("ServiceCatalogManager_GetCachedServiceCatalogs",
            //  s_businessEntityDefinitionId,
            //    () =>
            //    {
            Dictionary<Guid, ServiceCatalog> allServiceCatalogs = new Dictionary<Guid, ServiceCatalog>();
            // var neededColumns = new List<string> { "Service", "ActivationFee", "RecurringFee", "Deposit", "BankGuarantee", "AdvancedPaymentNbBillingCycles" };
            var businessEntities = genericBEManager.GetAllGenericBusinessEntities(s_businessEntityDefinitionId, null, null);

            if (businessEntities != null)
            {
                foreach (var entity in businessEntities)
                {
                    var catalog = new ServiceCatalog
                    {
                        ContractTypeId = (Guid)entity.FieldValues["ContractType"],
                        ServiceId = (Guid)entity.FieldValues["Service"],
                        SelectedByDefault = (bool)entity.FieldValues["SelectedByDefault"],
                        RequiredOption = (ServiceRequiredOption?)(int?)entity.FieldValues["RequiredOption"],
                        EligibleToSpecificTechnologies = (bool)entity.FieldValues["EligibleToSpecificTechnologies"]
                    };

                    if (catalog.EligibleToSpecificTechnologies)
                    {
                        dynamic eligibleTechnologies = entity.FieldValues["EligibleTechnologies"];
                        if (eligibleTechnologies != null)
                        {
                            catalog.EligibleTechnologyIds = new List<Guid>();
                            foreach (var eligibleTech in eligibleTechnologies)
                            {
                                catalog.EligibleTechnologyIds.Add(eligibleTech.AccessTechnology);
                            }
                        }
                    }

                    dynamic restrictedServiceGroups = entity.FieldValues["RestrictedServiceGroups"];
                    if (restrictedServiceGroups != null)
                    {
                        catalog.RestrictedServiceGroupIds = new List<Guid>();
                        foreach (var restritedServiceGroup in restrictedServiceGroups)
                        {
                            catalog.RestrictedServiceGroupIds.Add(restritedServiceGroup.Group);
                        }
                    }

                    allServiceCatalogs.Add(catalog.ServiceId, catalog);
                }
            }

            return allServiceCatalogs;
            //});
        }

        public bool AreServicesEligible(List<Guid> serviceIds, Guid contractTypeId, Guid technologyId, out List<ServiceEligibilityProblem> eligibilityProblems, out string eligibilityError)
        {
            List<Guid> restrictedGroupIds = new List<Guid>();
            eligibilityProblems = new List<ServiceEligibilityProblem>();
            System.Text.StringBuilder errorBuilder = new System.Text.StringBuilder();

            var serviceCatalogs = GetCachedServiceCatalogs();

            foreach (var serviceId in serviceIds)
            {
                var serviceCatalog = serviceCatalogs.GetRecord(serviceId);
                if (serviceCatalog == null)
                    continue;

                if (!IsServiceEligibleToTechnology(serviceCatalog, technologyId))
                {
                    if (!eligibilityProblems.Contains(ServiceEligibilityProblem.ServiceIsNotEligibleToTechnology))
                    {
                        eligibilityProblems.Add(ServiceEligibilityProblem.ServiceIsNotEligibleToTechnology);
                        errorBuilder.AppendLine("Some Services are not eligible to technology");
                    }
                }

                if (serviceCatalog.RestrictedServiceGroupIds != null && serviceCatalog.RestrictedServiceGroupIds.Count > 0)
                {
                    foreach (var groupId in serviceCatalog.RestrictedServiceGroupIds)
                    {
                        if (restrictedGroupIds.Contains(groupId))
                        {
                            if (!eligibilityProblems.Contains(ServiceEligibilityProblem.MultipleServicesBelongToSameRestrictedGroup))
                            {
                                eligibilityProblems.Add(ServiceEligibilityProblem.MultipleServicesBelongToSameRestrictedGroup);
                                errorBuilder.AppendLine("Multiple Services belong to same restricted Group");
                            }
                        }
                        else
                        {
                            restrictedGroupIds.Add(groupId);
                        }
                    }
                }
            }

            foreach (var serviceCatalog in serviceCatalogs.Values)
            {
                if (serviceCatalog.RequiredOption == ServiceRequiredOption.RequiredIfEligible
                    && serviceCatalog.ContractTypeId == contractTypeId
                    && !serviceIds.Contains(serviceCatalog.ServiceId))
                {
                    if (IsServiceEligibleToTechnology(serviceCatalog, technologyId))
                    {
                        if (!eligibilityProblems.Contains(ServiceEligibilityProblem.MissingRequiredService))
                        {
                            eligibilityProblems.Add(ServiceEligibilityProblem.MissingRequiredService);
                            errorBuilder.AppendLine("Missing Required Service");
                        }
                    }
                }
            }

            eligibilityError = errorBuilder.ToString();
            return eligibilityProblems.Count == 0;
        }


        public bool IsServiceEligible(Guid serviceId, Guid technologyId, out ServiceEligibilityInfo eligibilityInfo)
        {
            eligibilityInfo = new ServiceEligibilityInfo();
            var serviceCatalog = GetServiceCatalog(serviceId);

            if (serviceCatalog == null)
            {
                return true;
            }
            else
            {
                if (IsServiceEligibleToTechnology(serviceCatalog, technologyId))
                {
                    eligibilityInfo.IsSelectedByDefault = serviceCatalog.SelectedByDefault;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        bool IsServiceEligibleToTechnology(ServiceCatalog serviceCatalog, Guid technologyId)
        {
            return !serviceCatalog.EligibleToSpecificTechnologies
                     ||
                     (serviceCatalog.EligibleTechnologyIds != null && serviceCatalog.EligibleTechnologyIds.Contains(technologyId));
        }
    }

    public class ServiceCatalog
    {
        public Guid ContractTypeId { get; set; }

        public Guid ServiceId { get; set; }

        public bool SelectedByDefault { get; set; }

        public ServiceRequiredOption? RequiredOption { get; set; }

        public bool EligibleToSpecificTechnologies { get; set; }

        public List<Guid> EligibleTechnologyIds { get; set; }

        public List<Guid> RestrictedServiceGroupIds { get; set; }
    }

    public enum ServiceRequiredOption
    {
        RequiredIfEligible = 1
    }

    public enum ServiceEligibilityProblem
    {
        ServiceIsNotEligibleToTechnology = 1,
        MultipleServicesBelongToSameRestrictedGroup = 2,
        MissingRequiredService = 3
    }

    public class ServiceEligibilityInfo
    {
        public bool IsSelectedByDefault { get; set; }
    }

    public class CalculateRatePlanServicesPricingInput
    {
        public int RatePlanId { get; set; }

        public List<Guid> ServiceIds { get; set; }
    }

    public class CalculateRatePlanServicesPricingOutput
    {
        public Decimal TotalActivationFee { get; set; }

        public Decimal TotalDeposit { get; set; }

        public Decimal TotalPaymentInAdvance { get; set; }

        public Decimal TotalBankGuarantee { get; set; }
    }
}
