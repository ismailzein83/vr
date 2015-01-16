using System;
using System.Collections.Generic;

namespace TABS
{
    [Serializable]
    public class FaultTicket : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        public virtual int FaultTicketID { get; set; }
        public virtual CarrierAccount CarrierAccount { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual DateTime TicketDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual DateTime FromDate { get; set; }
        public virtual DateTime? ToDate { get; set; }
        public virtual string OwnReference { get; set; }
        public virtual string Reference { get; set; }
        public virtual TimeSpan AlertPeriod { get; set; }
        public virtual DateTime AlertTime { get; set; }
        public virtual int? Attempts { get; set; }
        public virtual float? ASR { get; set; }
        public virtual float? ACD { get; set; }
        public virtual string TestNumbers { get; set; }
        public virtual string FileName { get; set; }
        public virtual byte[] Attachment { get; set; }
        public virtual string SupportEmail
        {
            get
            {
                if (CarrierAccount.CarrierMask == "SYS")
                    return CarrierAccount.CarrierProfile.SupportEmail;
                return CarrierAccount.CustomerMaskAccount.CarrierProfile.SupportEmail;
            }
        }

        internal static IList<FaultTicketUpdate> _TicketUpdates;
        public IList<FaultTicketUpdate> TicketUpdates
        {
            get
            {
                if (_TicketUpdates == null)
                {
                    _TicketUpdates = ObjectAssembler.GetFaultTicketUpdates(this);
                }
                return _TicketUpdates;
            }
            set
            {
                _TicketUpdates = value;
            }
        }

        public override string Identifier { get { return "Fault Ticket:" + FaultTicketID; } }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(this.Identifier);
            return sb.ToString();
        }


        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _TicketUpdates = null;
            TABS.Components.CacheProvider.Clear(typeof(FaultTicket).FullName);
        }
    }
}