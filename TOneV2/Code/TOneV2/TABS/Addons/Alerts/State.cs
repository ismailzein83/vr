using System;
using System.Xml.Serialization;

namespace TABS.Addons.Alerts
{
    [Serializable]
    public class State : IXmlSerializable
    {
        public int Attempts { get; set; }
        public int DeliveredAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public decimal DurationsInSeconds { get; set; }
        public decimal MaxDurationInSeconds { get; set; }
        public decimal Average_PDD { get; set; }
        

        #region Calculated Fields

        public decimal Current_NER
        {
            get
            {
                if(Attempts > 0) 
                    return ((decimal)(100 * DeliveredAttempts / Attempts));
                else
                    return 0;
            }
        }

        public decimal Current_ASR
        {
            get
            {
                if (Attempts > 0) return ((decimal)(SuccessfulAttempts * 100) / Attempts);
                else return 0;
            }
        }

        public decimal Current_ACD
        {
            get
            {
                if (SuccessfulAttempts > 0) return DurationsInSeconds / SuccessfulAttempts / 60;
                else return 0;
            }
        }

        #endregion Calculated Fields

        public State() { Attempts = SuccessfulAttempts = 0; DurationsInSeconds = 0; MaxDurationInSeconds = 0; }
        public override string ToString()
        {
            return string.Format
                ("Attempts: {0} *-* Successful: {1} *-* Durations: {2:f2} *-* MaxDuration: {3:f2} *-* ASR: {4:f2} *-* ACD: {5:f2}",
                Attempts, SuccessfulAttempts, DurationsInSeconds / 60, MaxDurationInSeconds / 60, Current_ASR, Current_ACD);
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;

            reader.Read();
            Attempts = reader.ReadElementContentAsInt("Attempts", "");
            SuccessfulAttempts = reader.ReadElementContentAsInt("SuccessfulAttempts", "");
            DurationsInSeconds = reader.ReadElementContentAsDecimal("DurationsInSeconds", "");
            MaxDurationInSeconds = reader.ReadElementContentAsDecimal("MaxDurationInSeconds", "");
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Attempts", Attempts.ToString());
            writer.WriteElementString("SuccessfulAttempts", SuccessfulAttempts.ToString());
            writer.WriteElementString("DurationsInSeconds", DurationsInSeconds.ToString());
            writer.WriteElementString("MaxDurationInSeconds", MaxDurationInSeconds.ToString());
        }

        #endregion
    }
}
