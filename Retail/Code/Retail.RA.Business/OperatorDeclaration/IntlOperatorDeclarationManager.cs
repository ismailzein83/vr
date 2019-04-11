using System;
using System.Collections.Generic;
using System.Linq;
using Retail.RA.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class IntlOperatorDeclarationManager
    {
        static readonly Guid BeDefinitionId = new Guid("6E73A6E0-B3E2-4930-86ED-9EF509D56960");

        public IEnumerable<IntlOperatorDeclaration> GetAllOperatorDecalarations()
        {
            var cachedOperatorDeclarations = GetCachedOperatorDeclarations();
            cachedOperatorDeclarations.ThrowIfNull("cachedOperatorDeclarations");
            return cachedOperatorDeclarations;
        }

        public IEnumerable<IntlVoiceDeclarationService> GetVoiceDeclarationServices(List<PeriodDefinition> periodDefinitions, IEnumerable<long> filteredOperatorIds)
        {
            List<IntlVoiceDeclarationService> operatorDeclarations = new List<IntlVoiceDeclarationService>();
            Dictionary<int, List<IntlVoiceDeclarationService>> operatorDeclarationByPeriodId = GetCachedVoiceDeclaration();

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

        public IEnumerable<IntlVoiceDeclarationService> GetVoiceDeclarationServices(List<PeriodDefinition> periodDefinitions, TrafficDirection trafficDirection)
        {
            List<IntlVoiceDeclarationService> operatorDeclarations = new List<IntlVoiceDeclarationService>();
            Dictionary<int, List<IntlVoiceDeclarationService>> operatorDeclarationByPeriodId =
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

        public IEnumerable<IntlVoiceDeclarationService> GetVoiceDeclarationServices(List<PeriodDefinition> periodDefinitions, TrafficDirection trafficDirection, IEnumerable<long> filteredOperatorIds)
        {
            List<IntlVoiceDeclarationService> operatorDeclarations = new List<IntlVoiceDeclarationService>();
            Dictionary<int, List<IntlVoiceDeclarationService>> operatorDeclarationByPeriodId =
                trafficDirection == TrafficDirection.IN
                    ? GetCachedInVoiceDeclaration()
                    : GetCachedOutVoiceDeclaration();

            foreach (var periodDefinition in periodDefinitions)
            {
                if (operatorDeclarationByPeriodId.TryGetValue(periodDefinition.PeriodDefinitionId, out var operatorDeclaration))
                {
                    var filteredOperatorDeclaration = operatorDeclaration.FindAllRecords(item => filteredOperatorIds.Contains(item.OperatorId));
                    if (filteredOperatorDeclaration != null && filteredOperatorDeclaration.Any())
                        operatorDeclarations.AddRange(filteredOperatorDeclaration);
                }
            }
            return operatorDeclarations.OrderBy(item => item.OperatorId).ThenBy(item => item.PeriodDefinition.FromDate);
        }

        public IEnumerable<IntlSMSDeclarationService> GetSMSDeclarationServices(List<PeriodDefinition> periodDefinitions, IEnumerable<long> filteredOperatorIds)
        {
            List<IntlSMSDeclarationService> operatorDeclarations = new List<IntlSMSDeclarationService>();
            Dictionary<int, List<IntlSMSDeclarationService>> operatorDeclarationByPeriodId = GetCachedSMSDeclaration();

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

        public IEnumerable<IntlSMSDeclarationService> GetSMSDeclarationServices(List<PeriodDefinition> periodDefinitions, TrafficDirection trafficDirection)
        {
            List<IntlSMSDeclarationService> operatorDeclarations = new List<IntlSMSDeclarationService>();
            Dictionary<int, List<IntlSMSDeclarationService>> operatorDeclarationByPeriodId =
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

        public IEnumerable<IntlSMSDeclarationService> GetSMSDeclarationServices(List<PeriodDefinition> periodDefinitions, TrafficDirection trafficDirection, IEnumerable<long> filteredOperatorIds)
        {
            List<IntlSMSDeclarationService> operatorDeclarations = new List<IntlSMSDeclarationService>();
            Dictionary<int, List<IntlSMSDeclarationService>> operatorDeclarationByPeriodId =
                trafficDirection == TrafficDirection.IN
                    ? GetCachedInSMSDeclaration()
                    : GetCachedOutSMSDeclaration();

            foreach (var periodDefinition in periodDefinitions)
            {
                if (operatorDeclarationByPeriodId.TryGetValue(periodDefinition.PeriodDefinitionId, out var operatorDeclaration))
                {
                    var filteredOperatorDeclaration = operatorDeclaration.FindAllRecords(item => filteredOperatorIds.Contains(item.OperatorId));
                    if (filteredOperatorDeclaration != null && filteredOperatorDeclaration.Any())
                        operatorDeclarations.AddRange(filteredOperatorDeclaration);
                }
            }
            return operatorDeclarations.OrderBy(item => item.OperatorId).ThenBy(item => item.PeriodDefinition.FromDate);
        }


        #region Private Methhods
        private Dictionary<int, List<IntlVoiceDeclarationService>> GetCachedOutVoiceDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedOutVoiceDeclaration", BeDefinitionId,
                () =>
                {
                    List<IntlOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IntlVoiceDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IntlVoiceDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IntlVoice voiceDeclarationServiceSettings && voiceDeclarationServiceSettings.TrafficDirection == TrafficDirection.OUT)
                            {
                                List<IntlVoiceDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IntlVoiceDeclarationService
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
        private Dictionary<int, List<IntlVoiceDeclarationService>> GetCachedInVoiceDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedInVoiceDeclaration", BeDefinitionId,
                () =>
                {
                    List<IntlOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IntlVoiceDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IntlVoiceDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IntlVoice voiceDeclarationServiceSettings && voiceDeclarationServiceSettings.TrafficDirection == TrafficDirection.IN)
                            {
                                List<IntlVoiceDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IntlVoiceDeclarationService
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
        private Dictionary<int, List<IntlVoiceDeclarationService>> GetCachedVoiceDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedInVoiceDeclaration", BeDefinitionId,
                () =>
                {
                    List<IntlOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IntlVoiceDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IntlVoiceDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IntlVoice voiceDeclarationServiceSettings)
                            {
                                List<IntlVoiceDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IntlVoiceDeclarationService
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
        
        private Dictionary<int, List<IntlSMSDeclarationService>> GetCachedOutSMSDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedOutSMSDeclaration", BeDefinitionId,
                () =>
                {
                    List<IntlOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IntlSMSDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IntlSMSDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IntlSMS smsDeclarationServiceSettings && smsDeclarationServiceSettings.TrafficDirection == TrafficDirection.OUT)
                            {
                                List<IntlSMSDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IntlSMSDeclarationService
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
        private Dictionary<int, List<IntlSMSDeclarationService>> GetCachedInSMSDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedInSMSDeclaration", BeDefinitionId,
                () =>
                {
                    List<IntlOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IntlSMSDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IntlSMSDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IntlSMS smsDeclarationServiceSettings && smsDeclarationServiceSettings.TrafficDirection == TrafficDirection.IN)
                            {
                                List<IntlSMSDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IntlSMSDeclarationService
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
        private Dictionary<int, List<IntlSMSDeclarationService>> GetCachedSMSDeclaration()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedSMSDeclaration", BeDefinitionId,
                () =>
                {
                    List<IntlOperatorDeclaration> cachedOperatorDeclarations = GetCachedOperatorDeclarations();
                    Dictionary<int, List<IntlSMSDeclarationService>> operatorDeclarationByPeriod = new Dictionary<int, List<IntlSMSDeclarationService>>();
                    foreach (var operatorDeclaration in cachedOperatorDeclarations)
                    {
                        foreach (var operatorDeclarationService in operatorDeclaration.OperatorDeclarationServices.Services)
                        {
                            if (operatorDeclarationService.Settings is IntlSMS smsDeclarationServiceSettings)
                            {
                                List<IntlSMSDeclarationService> operatorDeclarations = operatorDeclarationByPeriod.GetOrCreateItem(operatorDeclaration.PeriodId);
                                operatorDeclarations.Add(new IntlSMSDeclarationService
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

        private List<IntlOperatorDeclaration> GetCachedOperatorDeclarations()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedOperatorDeclarations", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<IntlOperatorDeclaration> operatorDeclarations = new List<IntlOperatorDeclaration>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        IntlOperatorDeclaration operatorDeclaration = new IntlOperatorDeclaration
                        {
                            ID = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            OperatorId = (long)genericBusinessEntity.FieldValues.GetRecord("Operator"),
                            PeriodId = (int)genericBusinessEntity.FieldValues.GetRecord("Period"),
                            OperatorDeclarationServices = genericBusinessEntity.FieldValues.GetRecord("OperatorDeclarationServices") as IntlOperatorDeclarationServices
                        };
                        operatorDeclarations.Add(operatorDeclaration);
                    }
                }
                return operatorDeclarations;
            });
        }
        #endregion
    }

    public class IntlVoiceDeclarationService
    {
        public long ID { get; set; }
        public PeriodDefinition PeriodDefinition { get; set; }
        public long OperatorId { get; set; }
        public IntlVoice VoiceSettings { get; set; }
    }

    public class IntlSMSDeclarationService
    {
        public long ID { get; set; }
        public PeriodDefinition PeriodDefinition { get; set; }
        public long OperatorId { get; set; }
        public IntlSMS SMSSettings { get; set; }
    }
}
