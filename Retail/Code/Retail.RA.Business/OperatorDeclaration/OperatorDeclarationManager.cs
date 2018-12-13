using System;
using System.Linq;
using Vanrise.Common;
using Retail.RA.Entities;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class OperatorDeclarationManager
    {
        static readonly Guid BeDefinitionId = new Guid("6E73A6E0-B3E2-4930-86ED-9EF509D56960");

        public IEnumerable<OperatorDeclaration> GetAllOperatorDecalarations()
        {
            var cachedOperatorDeclarations = GetCachedOperatorDeclarations();
            cachedOperatorDeclarations.ThrowIfNull("cachedOperatorDeclarations");
            return cachedOperatorDeclarations;
        }

        public IEnumerable<VoiceDeclarationService> GetVoiceDeclarationServices(List<PeriodDefinition> periodDefinitions)
        {
            List<VoiceDeclarationService> operatorDeclarations = new List<VoiceDeclarationService>();
            Dictionary<int, List<VoiceDeclarationService>> operatorDeclarationByPeriodId = GetCachedInVoiceDeclaration();
            foreach (var periodDefinition in periodDefinitions)
            {
                if (operatorDeclarationByPeriodId.TryGetValue(periodDefinition.PeriodDefinitionId, out var operatorDeclaration))
                    operatorDeclarations.AddRange(operatorDeclaration);
            }
            return operatorDeclarations.OrderBy(item => item.OperatorId).ThenBy(item => item.PeriodDefinition.FromDate);
        }
        public VoiceDeclarationService GetOperatorDeclaration(long operatorId, int periodDefinitionId)
        {
            Dictionary<int, List<VoiceDeclarationService>> operatorDeclarationByPeriodId = GetCachedInVoiceDeclaration();

            if (operatorDeclarationByPeriodId == null || operatorDeclarationByPeriodId.Count == 0)
                return null;

            List<VoiceDeclarationService> operatorDeclarations = operatorDeclarationByPeriodId.GetRecord(periodDefinitionId);

            if (operatorDeclarations == null || operatorDeclarations.Count == 0)
                return null;

            return operatorDeclarations.FirstOrDefault(item => item.OperatorId == operatorId);
        }
        public Dictionary<int, List<VoiceDeclarationService>> GetCachedInVoiceDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedInVoiceDeclaration", BeDefinitionId,
                () =>
                {
                    List<OperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<VoiceDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<VoiceDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is Voice voiceDeclarationServiceSettings && voiceDeclarationServiceSettings.TrafficDirection == TrafficDirection.IN)
                            {
                                List<VoiceDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new VoiceDeclarationService
                                {
                                    ID = operatorDeclaration.ID,
                                    OperatorId = operatorDeclaration.OperatorId,
                                    PeriodDefinition = new PeriodDefinitionManager().GetPeriodDefinition(operatorDeclaration.PeriodId),
                                    VoiceSettings = voiceDeclarationServiceSettings
                                });
                            }
                        }

                    }
                    return operatorDeclarationByPeriod;
                });
        }

        private List<OperatorDeclaration> GetCachedOperatorDeclarations()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedOperatorDeclarations", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<OperatorDeclaration> operatorDeclarations = new List<OperatorDeclaration>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        OperatorDeclaration operatorDeclaration = new OperatorDeclaration
                        {
                            ID = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            OperatorId = (long)genericBusinessEntity.FieldValues.GetRecord("Operator"),
                            PeriodId = (int)genericBusinessEntity.FieldValues.GetRecord("Period"),
                            OperatorDeclarationServices = genericBusinessEntity.FieldValues.GetRecord("OperatorDeclarationServices") as OperatorDeclarationServices
                        };
                        operatorDeclarations.Add(operatorDeclaration);
                    }
                }
                return operatorDeclarations;
            });
        }
    }

    public class VoiceDeclarationService
    {
        public long ID { get; set; }
        public PeriodDefinition PeriodDefinition { get; set; }
        public long OperatorId { get; set; }
        public Voice VoiceSettings { get; set; }
    }
}
