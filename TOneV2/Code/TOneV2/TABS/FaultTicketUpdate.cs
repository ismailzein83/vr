using System;
using System.Text;

namespace TABS
{
    public class FaultTicketUpdate : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        public virtual int FaultTicketUpdateID { get; set; }
        public virtual bool SendMail { get; set; }
        public virtual String Email { get; set; }
        public virtual String Contact { get; set; }
        public virtual String PhoneNo { get; set; }
        public virtual String Notes { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual string FileName { get; set; }
        public virtual byte[] Attachment { get; set; }
        public virtual FaultTicket FaultTicket { get; set; }


        public override string Identifier { get { return "Fault Ticket History :" + FaultTicketUpdateID; } }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Ticket Update for: ");
            sb.Append(this.Identifier);
            return sb.ToString();
        }

        public string NumberFormatted
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.FaultTicket.TicketUpdates.Count > 1) sb.Append("");
                else
                {
                    sb.Append("Details:<br />");
                    sb.AppendFormat("Time and Date of Testing:  {0} GMT  -  {1} GMT <br />", this.FaultTicket.FromDate.ToString("dd/MM/yyyy HH:mm"), this.FaultTicket.ToDate.Value.ToString("dd/MM/yyyy HH:mm"));

                    if (!string.IsNullOrEmpty(this.FaultTicket.TestNumbers))
                    {
                        sb.Append("<br />Tested Numbers: <br />");
                        string[] numbers = this.FaultTicket.TestNumbers.Split(';');
                        int j = 1;
                        do
                        {
                            sb.AppendFormat("{0} - {1}<br />", j, numbers[j - 1]);
                            j++;
                        } while (j < numbers.Length);
                    }
                    if (this.FaultTicket.Attempts != null)
                    {
                        sb.AppendFormat("<br />Attempts:{0} <br />", this.FaultTicket.Attempts);
                    }
                    if (this.FaultTicket.ASR != null)
                    {
                        sb.AppendFormat("ASR:{0} <br />", this.FaultTicket.ASR);
                    }
                    if (this.FaultTicket.ACD != null)
                    {
                        sb.AppendFormat("ACD:{0} <br />", this.FaultTicket.ACD);
                    }
                }
                return sb.ToString();
            }
        }

        public string NoteMail
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.FaultTicket.TicketType == TicketType.IN && this.FaultTicket.TicketUpdates.Count < 2)
                {
                    sb.AppendFormat(@"Kindly open a trouble ticket for the fault described below:
                                  <br />Destination: {0}
                                  <br />{1}
                                  <br />{2}  
                                  <br />Ticket no: {3}
                                  <br />============================ Best regards ============================",
                           this.FaultTicket.Zone.Name,
                           this.NumberFormatted,
                           string.IsNullOrEmpty(this.Notes) ? "" : this.Notes,
                           this.FaultTicket.FaultTicketID
                           );
                }
                else
                {
                    sb.AppendFormat(@"{0}", string.IsNullOrEmpty(this.Notes) ? "" : this.Notes);
                }
                return sb.ToString();
            }
        }

        public string FaultTicketOwnReference { get { return FaultTicket.OwnReference; } }
        public string FaultTicketASR { get { return (FaultTicket.ASR != null) ? FaultTicket.ASR.ToString() : "not available"; } }
        public string FaultTicketACD { get { return (FaultTicket.ACD != null) ? FaultTicket.ACD.ToString() : "not available"; } }
        public string FaultTicketAttempts { get { return (FaultTicket.Attempts != null) ? FaultTicket.Attempts.ToString() : "not available"; } }
        public string FaultTicketNumbers
        {
            get
            {
                StringBuilder sb = new StringBuilder("");
                if (!string.IsNullOrEmpty(FaultTicket.TestNumbers))
                {
                    string[] numbers = FaultTicket.TestNumbers.Split(';');
                    int j = 1;
                    do
                    {
                        sb.AppendFormat("{0} <br/>", numbers[j - 1]).AppendLine();
                        j++;
                    } while (j < numbers.Length);
                }
                return sb.ToString();
            }
        }
        public string FaultTicketUpdateNotes
        {
            get
            {
                return Notes.Replace("\r\n", "<br/>").ToString();
            }
        }

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            TABS.Components.CacheProvider.Clear(typeof(FaultTicketUpdate).FullName);
        }

    }
}
