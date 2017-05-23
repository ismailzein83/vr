using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            List<VREventHandler> eventHandlers = GetEventHandlers(eventTypeUniqueName);
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

        private List<VREventHandler> GetEventHandlers(string eventTypeUniqueName)
        {
            throw new NotImplementedException();
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


        #endregion
    }
}
