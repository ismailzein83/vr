using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class OnNetOperatorDeclarationManager
    {
        static readonly Guid beDefinitionId = new Guid("b4a7a7f9-8912-40a5-a985-73a44bdf82bc");
        #region Public Methods
        public IEnumerable<OnNetOperatorDeclaration> GetAllOperatorDeclarations()
        {
            var cachedOperatorDeclarations = GetCachedOperatorDeclarations();
            cachedOperatorDeclarations.ThrowIfNull("cachedOperatorDeclarations");
            return cachedOperatorDeclarations;
        }

        public IEnumerable<OnNetDeclarationService> GetDeclarationServices(List<PeriodDefinition> periodDefinitions, IEnumerable<long> filteredOperatorIds)
        {
            List<OnNetDeclarationService> operatorDeclarations = new List<OnNetDeclarationService>();
            if (periodDefinitions != null && periodDefinitions.Count > 0)
            {
                Dictionary<int, List<OnNetDeclarationService>> operatorDeclarationByPeriodId = GetCachedDeclarationServices();
                foreach (var periodDefinition in periodDefinitions)
                {
                    if (operatorDeclarationByPeriodId.TryGetValue(periodDefinition.PeriodDefinitionId, out var operatorDeclaration))
                    {
                        var filteredOperatorDeclaration = operatorDeclaration.FindAllRecords(item => filteredOperatorIds == null || filteredOperatorIds.Contains(item.OperatorId));
                        if (filteredOperatorDeclaration != null && filteredOperatorDeclaration.Count() > 0)
                            operatorDeclarations.AddRange(filteredOperatorDeclaration);
                    }
                }
            }
            return operatorDeclarations.OrderBy(item => item.OperatorId).ThenBy(item => item.PeriodDefinition.FromDate);
        }
        #endregion

        #region Private Methods


        private Dictionary<int, List<OnNetDeclarationService>> GetCachedDeclarationServices()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedDeclarationServices", beDefinitionId,
                () =>
                {
                    List<OnNetOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<OnNetDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<OnNetDeclarationService>>();
                    if (cachedOperatorDeclarations != null && cachedOperatorDeclarations.Count > 0)
                    {
                        foreach (var operatorDeclaration in cachedOperatorDeclarations)
                        {
                            if (operatorDeclaration.OperatorDeclarationServices != null && operatorDeclaration.OperatorDeclarationServices.Services != null)
                            {
                                foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                                {
                                    List<OnNetDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodID);
                                    operatorDeclarations.Add(new OnNetDeclarationService
                                    {
                                        ID = operatorDeclaration.ID,
                                        OperatorId = operatorDeclaration.OperatorID,
                                        PeriodDefinition = new PeriodDefinitionManager().GetPeriodDefinition(operatorDeclaration.PeriodID),
                                        Settings = operatorDeclarationService.Settings
                                    });
                                }
                            }
                        }
                    }
                    return operatorDeclarationByPeriod;
                });
        }

        private List<OnNetOperatorDeclaration> GetCachedOperatorDeclarations()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedOperatorDeclarations", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                List<OnNetOperatorDeclaration> operatorDeclarations = new List<OnNetOperatorDeclaration>();

                if (genericBusinessEntities != null && genericBusinessEntities.Count > 0)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        OnNetOperatorDeclaration operatorDeclaration = new OnNetOperatorDeclaration
                        {
                            ID = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            OperatorID = (long)genericBusinessEntity.FieldValues.GetRecord("Operator"),
                            PeriodID = (int)genericBusinessEntity.FieldValues.GetRecord("Period"),
                            OperatorDeclarationServices = genericBusinessEntity.FieldValues.GetRecord("OperatorDeclarationServices") as OnNetOperatorDeclarationServices
                        };
                        operatorDeclarations.Add(operatorDeclaration);
                    }
                }
                return operatorDeclarations;
            });
        }

        #endregion
    }
}
