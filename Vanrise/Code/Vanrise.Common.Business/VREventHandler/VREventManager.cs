using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VREventManager
    {
        /// <summary>
        /// Should be called On Before Event execution
        /// </summary>
        /// <param name="eventPayload"></param>
        public void ExecuteEventHandlersSync(VREventPayload eventPayload)
        {
            string eventTypeUniqueName = eventPayload.GetEventTypeUniqueName();
            eventTypeUniqueName.ThrowIfNull("eventTypeUniqueName");
            IEnumerable<VREventHandler> eventHandlers = GetEventHandlers(eventTypeUniqueName);
            if(eventHandlers != null)
            {
                foreach(var eventHandler in eventHandlers)
                {
                    eventHandler.Settings.ThrowIfNull("eventHandler.Settings", eventHandler.VREventHandlerId);
                    eventHandler.Settings.ExtendedSettings.ThrowIfNull("eventHandler.Settings.ExtendedSettings", eventHandler.VREventHandlerId);
                    if(eventHandler.IsEffective(DateTime.Now))
                    {
                        var context = new VREventHandlerContext { EventPayload = eventPayload };
                        eventHandler.Settings.ExtendedSettings.Execute(context);
                    }
                }
            }
        }

        /// <summary>
        /// Should be called After Event execution
        /// </summary>
        /// <param name="eventPayload"></param>
        public void ExecuteEventHandlersAsync(VREventPayload eventPayload)
        {
            //this method should be refactored to queue the event payload and execute the handlers asynchronously
            ExecuteEventHandlersSync(eventPayload);
        }

        public VREventHandler GetVREventHandler(Guid vREventHandlerId)
        {
            Dictionary<Guid, VREventHandler> cachedVREventHandlers = this.GetCachedVREventHandlers();
            return cachedVREventHandlers.GetRecord(vREventHandlerId);
        }
        public string GetVREventHandlerName(Guid vREventHandlerId)
        {
            VREventHandler vREventHandler = this.GetVREventHandler(vREventHandlerId);
            return (vREventHandler != null) ? vREventHandler.Name : null;
        }

        public IDataRetrievalResult<VREventHandlerDetail> GetFilteredVREventHandlers(DataRetrievalInput<VREventHandlerQuery> input)
        {
            var allVREventHandlers = GetCachedVREventHandlers();
            Func<VREventHandler, bool> filterExpression = (x) =>
            {
                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VREventHandlerLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVREventHandlers.ToBigResult(input, filterExpression, VREventHandlerDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<VREventHandlerDetail> AddVREventHandler(VREventHandler vREventHandlerItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VREventHandlerDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVREventHandlerDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVREventHandlerDataManager>();

            vREventHandlerItem.VREventHandlerId = Guid.NewGuid();

            if (dataManager.Insert(vREventHandlerItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VREventHandlerLoggableEntity.Instance, vREventHandlerItem);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VREventHandlerDetailMapper(vREventHandlerItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<VREventHandlerDetail> UpdateVREventHandler(VREventHandler vREventHandlerItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VREventHandlerDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVREventHandlerDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVREventHandlerDataManager>();

            if (dataManager.Update(vREventHandlerItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VREventHandlerLoggableEntity.Instance, vREventHandlerItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VREventHandlerDetailMapper(this.GetVREventHandler(vREventHandlerItem.VREventHandlerId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<VREventHandlerInfo> GetVREventHandlersInfo(VREventHandlerInfoFilter filter)
        {
            Func<VREventHandler, bool> filterExpression = null;

            if (filter != null)
            {
                filterExpression = (x) =>
                {
                    return true;
                };
            }
            return this.GetCachedVREventHandlers().MapRecords(VREventHandlerInfoMapper, filterExpression).OrderBy(x => x.Name);
        }
        private IEnumerable<VREventHandler> GetEventHandlers(string eventTypeUniqueName)
        {
            return this.GetCachedVREventHandlers().FindAllRecords(x => x.Settings.ExtendedSettings.GetEventTypeUniqueName() == eventTypeUniqueName).OrderBy(x => x.Name);
        }

        #region Private Classes

        private class VREventHandlerContext : IVREventHandlerContext
        {
            public VREventPayload EventPayload
            {
                get;
                set;
            }
        }
        private class VREventHandlerLoggableEntity : VRLoggableEntityBase
        {
            public static VREventHandlerLoggableEntity Instance = new VREventHandlerLoggableEntity();

            private VREventHandlerLoggableEntity()
            {

            }

            static VREventManager s_VREventHandlerManager = new VREventManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_VREventHandler"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Event Handler"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_VREventHandler_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VREventHandler vREventHandler = context.Object.CastWithValidate<VREventHandler>("context.Object");
                return vREventHandler.VREventHandlerId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VREventHandler vREventHandler = context.Object.CastWithValidate<VREventHandler>("context.Object");
                return s_VREventHandlerManager.GetVREventHandlerName(vREventHandler.VREventHandlerId);
            }
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVREventHandlerDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVREventHandlerDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVREventHandlerUpdated(ref _updateHandle);
            }
        }


        #endregion
     
        #region Private Methods
        Dictionary<Guid, VREventHandler> GetCachedVREventHandlers()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVREventHandlers",
               () =>
               {
                   IVREventHandlerDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVREventHandlerDataManager>();
                   return dataManager.GetVREventHandlers().ToDictionary(x => x.VREventHandlerId, x => x);
               });
        }
        #endregion


        #region Mappers

        public VREventHandlerDetail VREventHandlerDetailMapper(VREventHandler vREventHandler)
        {
            throw new NotImplementedException();
        }

        public VREventHandlerInfo VREventHandlerInfoMapper(VREventHandler vREventHandler)
        {
            VREventHandlerInfo vREventHandlerInfo = new VREventHandlerInfo()
            {
                VREventHandlerId = vREventHandler.VREventHandlerId,
                Name = vREventHandler.Name
            };
            return vREventHandlerInfo;
        }
       
        #endregion

    }
}
