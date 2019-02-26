using System;
using System.Linq;
using Vanrise.Common;
using Retail.RA.Entities;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class IcxOperatorDeclarationManager
    {
        static readonly Guid BeDefinitionId = new Guid("8DEF6E27-D9BC-4CB4-863A-972DD735B63F");

        public IEnumerable<IcxOperatorDeclaration> GetAllOperatorDecalarations()
        {
            var cachedOperatorDeclarations = GetCachedOperatorDeclarations();
            cachedOperatorDeclarations.ThrowIfNull("cachedOperatorDeclarations");
            return cachedOperatorDeclarations;
        }

        public IEnumerable<IcxVoiceDeclarationService> GetVoiceDeclarationServices(List<PeriodDefinition> periodDefinitions, IEnumerable<long> filteredOperatorIds)
        {
            List<IcxVoiceDeclarationService> operatorDeclarations = new List<IcxVoiceDeclarationService>();
            Dictionary<int, List<IcxVoiceDeclarationService>> operatorDeclarationByPeriodId = GetCachedVoiceDeclaration();

            foreach (var periodDefinition in periodDefinitions)
            {
                if (operatorDeclarationByPeriodId.TryGetValue(periodDefinition.PeriodDefinitionId, out var operatorDeclaration))
                {
                    var filteredOperatorDeclaration = operatorDeclaration.FindAllRecords(item => filteredOperatorIds == null || filteredOperatorIds.Contains(item.OperatorId));
                    if (filteredOperatorDeclaration != null && filteredOperatorDeclaration.Any())
                        operatorDeclarations.AddRange(filteredOperatorDeclaration);
                }
            }
            return operatorDeclarations.OrderBy(item => item.OperatorId).ThenBy(item => item.PeriodDefinition.FromDate);
        }

        public IEnumerable<IcxVoiceDeclarationService> GetVoiceDeclarationServices(List<PeriodDefinition> periodDefinitions, TrafficDirection trafficDirection)
        {
            List<IcxVoiceDeclarationService> operatorDeclarations = new List<IcxVoiceDeclarationService>();
            Dictionary<int, List<IcxVoiceDeclarationService>> operatorDeclarationByPeriodId =
                trafficDirection == TrafficDirection.IN
                    ? GetCachedInVoiceDeclaration()
                    : GetCachedOutVoiceDeclaration();

            foreach (var periodDefinition in periodDefinitions)
            {
                if (operatorDeclarationByPeriodId.TryGetValue(periodDefinition.PeriodDefinitionId, out var operatorDeclaration))
                    operatorDeclarations.AddRange(operatorDeclaration);
            }
            return operatorDeclarations.OrderBy(item => item.OperatorId).ThenBy(item => item.PeriodDefinition.FromDate);
        }

        public IEnumerable<IcxSMSDeclarationService> GetSMSDeclarationServices(List<PeriodDefinition> periodDefinitions, IEnumerable<long> filteredOperatorIds)
        {
            List<IcxSMSDeclarationService> operatorDeclarations = new List<IcxSMSDeclarationService>();
            Dictionary<int, List<IcxSMSDeclarationService>> operatorDeclarationByPeriodId = GetCachedSMSDeclaration();

            foreach (var periodDefinition in periodDefinitions)
            {
                if (operatorDeclarationByPeriodId.TryGetValue(periodDefinition.PeriodDefinitionId, out var operatorDeclaration))
                {
                    var filteredOperatorDeclaration = operatorDeclaration.FindAllRecords(item => filteredOperatorIds == null || filteredOperatorIds.Contains(item.OperatorId));
                    if (filteredOperatorDeclaration != null && filteredOperatorDeclaration.Any())
                        operatorDeclarations.AddRange(filteredOperatorDeclaration);
                }
            }
            return operatorDeclarations.OrderBy(item => item.OperatorId).ThenBy(item => item.PeriodDefinition.FromDate);
        }

        public IEnumerable<IcxSMSDeclarationService> GetSMSDeclarationServices(List<PeriodDefinition> periodDefinitions, TrafficDirection trafficDirection)
        {
            List<IcxSMSDeclarationService> operatorDeclarations = new List<IcxSMSDeclarationService>();
            Dictionary<int, List<IcxSMSDeclarationService>> operatorDeclarationByPeriodId =
                trafficDirection == TrafficDirection.IN
                    ? GetCachedInSMSDeclaration()
                    : GetCachedOutSMSDeclaration();

            foreach (var periodDefinition in periodDefinitions)
            {
                if (operatorDeclarationByPeriodId.TryGetValue(periodDefinition.PeriodDefinitionId, out var operatorDeclaration))
                    operatorDeclarations.AddRange(operatorDeclaration);
            }
            return operatorDeclarations.OrderBy(item => item.OperatorId).ThenBy(item => item.PeriodDefinition.FromDate);
        }


        #region Private Methhods

        private Dictionary<int, List<IcxVoiceDeclarationService>> GetCachedVoiceDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedInVoiceDeclaration", BeDefinitionId,
                () =>
                {
                    List<IcxOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IcxVoiceDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IcxVoiceDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IcxVoice voiceDeclarationServiceSettings)
                            {
                                List<IcxVoiceDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IcxVoiceDeclarationService
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

        private Dictionary<int, List<IcxVoiceDeclarationService>> GetCachedOutVoiceDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedOutVoiceDeclaration", BeDefinitionId,
                () =>
                {
                    List<IcxOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IcxVoiceDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IcxVoiceDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IcxVoice voiceDeclarationServiceSettings && voiceDeclarationServiceSettings.TrafficDirection == TrafficDirection.OUT)
                            {
                                List<IcxVoiceDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IcxVoiceDeclarationService
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
        private Dictionary<int, List<IcxVoiceDeclarationService>> GetCachedInVoiceDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedInVoiceDeclaration", BeDefinitionId,
                () =>
                {
                    List<IcxOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IcxVoiceDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IcxVoiceDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IcxVoice voiceDeclarationServiceSettings && voiceDeclarationServiceSettings.TrafficDirection == TrafficDirection.IN)
                            {
                                List<IcxVoiceDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IcxVoiceDeclarationService
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


        private Dictionary<int, List<IcxSMSDeclarationService>> GetCachedOutSMSDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedOutSMSDeclaration", BeDefinitionId,
                () =>
                {
                    List<IcxOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IcxSMSDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IcxSMSDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IcxSMS smsDeclarationServiceSettings && smsDeclarationServiceSettings.TrafficDirection == TrafficDirection.OUT)
                            {
                                List<IcxSMSDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IcxSMSDeclarationService
                                {
                                    ID = operatorDeclaration.ID,
                                    OperatorId = operatorDeclaration.OperatorId,
                                    PeriodDefinition = new PeriodDefinitionManager().GetPeriodDefinition(operatorDeclaration.PeriodId),
                                    SMSSettings = smsDeclarationServiceSettings
                                });
                            }
                        }

                    }
                    return operatorDeclarationByPeriod;
                });
        }
        private Dictionary<int, List<IcxSMSDeclarationService>> GetCachedInSMSDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedInSMSDeclaration", BeDefinitionId,
                () =>
                {
                    List<IcxOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IcxSMSDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IcxSMSDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IcxSMS smsDeclarationServiceSettings && smsDeclarationServiceSettings.TrafficDirection == TrafficDirection.IN)
                            {
                                List<IcxSMSDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IcxSMSDeclarationService
                                {
                                    ID = operatorDeclaration.ID,
                                    OperatorId = operatorDeclaration.OperatorId,
                                    PeriodDefinition = new PeriodDefinitionManager().GetPeriodDefinition(operatorDeclaration.PeriodId),
                                    SMSSettings = smsDeclarationServiceSettings
                                });
                            }
                        }

                    }
                    return operatorDeclarationByPeriod;
                });
        }
        private Dictionary<int, List<IcxSMSDeclarationService>> GetCachedSMSDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedSMSDeclaration", BeDefinitionId,
                () =>
                {
                    List<IcxOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IcxSMSDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IcxSMSDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IcxSMS smsDeclarationServiceSettings)
                            {
                                List<IcxSMSDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IcxSMSDeclarationService
                                {
                                    ID = operatorDeclaration.ID,
                                    OperatorId = operatorDeclaration.OperatorId,
                                    PeriodDefinition = new PeriodDefinitionManager().GetPeriodDefinition(operatorDeclaration.PeriodId),
                                    SMSSettings = smsDeclarationServiceSettings
                                });
                            }
                        }

                    }
                    return operatorDeclarationByPeriod;
                });
        }

        private List<IcxOperatorDeclaration> GetCachedOperatorDeclarations()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedOperatorDeclarations", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<IcxOperatorDeclaration> operatorDeclarations = new List<IcxOperatorDeclaration>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        IcxOperatorDeclaration operatorDeclaration = new IcxOperatorDeclaration
                        {
                            ID = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            OperatorId = (long)genericBusinessEntity.FieldValues.GetRecord("Operator"),
                            PeriodId = (int)genericBusinessEntity.FieldValues.GetRecord("Period"),
                            OperatorDeclarationServices = genericBusinessEntity.FieldValues.GetRecord("OperatorDeclarationServices") as IcxOperatorDeclarationServices
                        };
                        operatorDeclarations.Add(operatorDeclaration);
                    }
                }
                return operatorDeclarations;
            });
        }
        #endregion
    }

    public class IcxVoiceDeclarationService
    {
        public long ID { get; set; }
        public PeriodDefinition PeriodDefinition { get; set; }
        public long OperatorId { get; set; }
        public IcxVoice VoiceSettings { get; set; }
    }

    public class IcxSMSDeclarationService
    {
        public long ID { get; set; }
        public PeriodDefinition PeriodDefinition { get; set; }
        public long OperatorId { get; set; }
        public IcxSMS SMSSettings { get; set; }
    }
}
