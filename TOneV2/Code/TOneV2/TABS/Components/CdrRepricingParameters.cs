using System;
using System.Collections.Generic;

namespace TABS.Components
{
    public class CdrRepricingParameters
    {
        #region Repricing Properties
        public DateTime From { get; set; }
        public DateTime Till { get; set; }
        public Switch SelectedSwitch { get; set; }
        public CarrierAccount Customer { get; set; }
        public CarrierAccount Supplier { get; set; }
        public int BatchSize { get; set; }
        public int DailyChunks { get; set; }
        public TimeSpan DailyChunkTime { get { return TimeSpan.FromTicks(TimeSpan.FromDays(1).Ticks / DailyChunks); } }
        public bool GenerateTrafficStats { get; set; }
        #endregion Repricing Properties

        #region Control Properties
        /// <summary>
        /// Gets or Sets whether a stop is requested or not
        /// </summary>
        public bool IsStopRequested { get; set; }

        /// <summary>
        /// The User that Laucnhed the repricing process
        /// </summary>
        public SecurityEssentials.User User { get; set; }

        /// <summary>
        /// The Date/Time when 
        /// </summary>
        public DateTime Created { get; protected set; }

        /// <summary>
        /// The Date/Time when 
        /// </summary>
        public DateTime? Started { get; internal set; }

        /// <summary>
        /// How long has repricing been running since Started.
        /// </summary>
        public TimeSpan Duration { get { return Started.HasValue ? DateTime.Now.Subtract(Started.Value) : TimeSpan.Zero; } }

        #endregion Control Properties

        public CdrRepricingParameters()
        {
            Created = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("Repricing: {0:yyyy-MM-dd} - {1:yyyy-MM-dd}, Customer: {2}, Supplier: {3}. Batch: {4}. Chunk: {5}. Stats: {6}"
                + "\r\n<br/>Created by: {7} on {8:yyyy-MM-dd HH:mm}, Started: {9:yyyy-MM-dd HH:mm}, duration: {10}.{11}"
                , this.From
                , this.Till
                , this.Customer
                , this.Supplier
                , this.BatchSize
                , this.DailyChunkTime
                , this.GenerateTrafficStats
                , this.User
                , this.Created
                , this.Started
                , this.Duration
                , this.IsStopRequested ? " <b style='color:red'>Stop Requested</b>" : ""
                );
        }

        public static Queue<CdrRepricingParameters> Queue = new Queue<CdrRepricingParameters>();

        public static void Add(CdrRepricingParameters repricingParameters)
        {
            lock (Queue)
            {
                Queue.Enqueue(repricingParameters);
            }
        }

        public static void Remove(CdrRepricingParameters repricingParameters)
        {
            lock (Queue)
            {
                List<CdrRepricingParameters> list = new List<CdrRepricingParameters>(Queue);
                list.Remove(repricingParameters);
                Queue = new Queue<CdrRepricingParameters>(list);
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
