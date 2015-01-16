using System;
using System.Collections.Generic;

namespace TABS.Components
{
    /// <summary>
    /// A logging event appender that holds a sepcified count (Capacity) of logging events.
    /// </summary>
    public class MemoryEventAppender : log4net.Appender.IAppender
    {
        protected static MemoryEventAppender instance;
        
        protected static NGenerics.DataStructures.Queues.CircularQueue<log4net.Core.LoggingEvent> _Events
                = new NGenerics.DataStructures.Queues.CircularQueue<log4net.Core.LoggingEvent>(DefaultCapacity);

        protected static NGenerics.DataStructures.Queues.CircularQueue<log4net.Core.LoggingEvent> _Errors
                = new NGenerics.DataStructures.Queues.CircularQueue<log4net.Core.LoggingEvent>(DefaultCapacity);

        /// <summary>
        /// Clear all the recorded events and errors
        /// </summary>
        public static void Clear()
        {
            _Events.Clear();
            _Errors.Clear();
        }

        /// <summary>
        /// Get or Set the event queue capacity.
        /// When the capacity the currently held events are lost.
        /// </summary>
        public static int Capacity
        {
            get
            {
                return _Events.Capacity;
            }
            set
            {
                _Events = new NGenerics.DataStructures.Queues.CircularQueue<log4net.Core.LoggingEvent>(value);
                _Errors = new NGenerics.DataStructures.Queues.CircularQueue<log4net.Core.LoggingEvent>(value);
            }
        }
        
        /// <summary>
        /// Returns a snapshot of the events in memory, last one first.
        /// </summary>
        public static IList<log4net.Core.LoggingEvent> Events 
        { 
            get
            {
                List<log4net.Core.LoggingEvent> events = new List<log4net.Core.LoggingEvent>(_Events);
                events.Reverse();
                return events; 
            }
        }

        /// <summary>
        /// Returns a snapshot of the errors in memory, last one first.
        /// </summary>
        public static IList<log4net.Core.LoggingEvent> Errors
        {
            get
            {
                List<log4net.Core.LoggingEvent> errors = new List<log4net.Core.LoggingEvent>(_Errors);
                errors.Reverse();
                return errors;
            }
        }

        /// <summary>
        /// Gets the default capacity for the events held in memory
        /// </summary>
        protected static int DefaultCapacity
        {
            get
            {
                int defaultCapacity = 10000;
                try
                {
                    defaultCapacity = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TABS.Components.MemoryAppender.DefaultCapacity"]);
                }
                catch
                {

                }
                return defaultCapacity;
            }
        }

        /// <summary>
        /// Constructor for the Memory Appender
        /// </summary>
        public MemoryEventAppender()
        {
            if (instance != null) throw new InvalidOperationException("There can be only one Memory Event Appender per application");
            else
                instance = this;
        }

        #region IAppender Members

        /// <summary>
        /// Close the appender.
        /// </summary>
        public void Close()
        {
            _Errors.Clear();
            _Events.Clear();
        }

        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            if(loggingEvent.ExceptionObject != null)
                _Errors.Enqueue(loggingEvent);
            else
                _Events.Enqueue(loggingEvent);
        }

        string _Name = "T.One - Memory Appender";

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        #endregion
    }
}
