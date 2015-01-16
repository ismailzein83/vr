using System;
using System.Collections.Generic;

namespace TABS.SpecialSystemParameters
{
    public class PostPaidCarrierOptions
    {
        /// <summary>
        /// A container for a Carrier option
        /// </summary>
        public class CarrierOption : IComparable<CarrierOption>
        {
            /// <summary>
            /// The Carrier this option was set for
            /// </summary>
            public CarrierAccount Carrier { get; set; }

            /// <summary>
            /// The percentage beginning from which the actions defined take place
            /// </summary>
            public decimal Percentage { get; set; }

            /// <summary>
            /// Bitwise Ored Actions (Can also be the sum)
            /// </summary>
            public EventActions Actions { get; set; }
            public Boolean IsCustommer { get; set; }
            /// <summary>
            /// Two Carrier options are logically equal if they are for the Carrier on the same percentage
            /// </summary>
            /// <returns>True if other object is logically equal this one</returns>
            public override bool Equals(object obj)
            {
                CarrierOption other = obj as CarrierOption;
                if (other != null)
                    return (this.Carrier.Equals(other.Carrier) && this.Percentage.Equals(other.Percentage))&& this.IsCustommer.Equals(other.IsCustommer);
                else 
                    return false;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return (this.Carrier.ToString() + ":" + this.Percentage.ToString()).GetHashCode();
            }

            #region IComparable<CarrierOption> Members

            public int CompareTo(CarrierOption other)
            {
                if (this.Carrier.Equals(other.Carrier))
                    return this.Percentage.CompareTo(other.Percentage);
                else
                    return this.Carrier.Name.CompareTo(other.Carrier.Name);
            }


            #endregion
        }

        /// <summary>
        /// The Dictionary of Carrier Options
        /// </summary>
        public Dictionary<CarrierAccount, List<CarrierOption>> CarrierOptions = new Dictionary<CarrierAccount, List<CarrierOption>>();

        /// <summary>
        /// Create the post paid options from the system parameter xml
        /// </summary>
        public PostPaidCarrierOptions()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            document.LoadXml(SystemParameter.PostPaidCarrierOptions.LongTextValue);
            LoadXml(document);
        }

        /// <summary>
        /// Return a sorted list (by Carrier, then by percentage) of all options
        /// </summary>
        /// <returns></returns>
        public List<CarrierOption> List(Boolean IsCustommer)
        {
            List<CarrierOption> allList = new List<CarrierOption>();

            foreach (CarrierAccount Carrier in this.CarrierOptions.Keys)
                foreach (CarrierOption option in this.CarrierOptions[Carrier])
                    if (option.IsCustommer == IsCustommer)
                    allList.Add(option);

            allList.Sort();

            return allList;
        }

        /// <summary>
        /// Load the Xml Document into this object and create related objects
        /// </summary>
        /// <param name="document"></param>
        protected void LoadXml(System.Xml.XmlDocument document)
        {
            // Clear internal dictionary
            this.CarrierOptions.Clear();

            System.Xml.XmlNodeList CarrierNodes = document.DocumentElement.SelectNodes("Carrier");

            // for each Carrier node create a dictionary key and load the options as objects
            foreach (System.Xml.XmlNode CarrierNode in CarrierNodes)
            {
                string carrierAccountID = CarrierNode.Attributes["CarrierAccountID"].Value;
                if (carrierAccountID != null && CarrierAccount.All.ContainsKey(carrierAccountID))
                {
                    CarrierAccount Carrier = CarrierAccount.All[carrierAccountID];
                    List<CarrierOption> CarrierOptionList = new List<CarrierOption>();

                    //this.CarrierOptions[Carrier] = CarrierOptionList;

                    // for each child node (option) create an object and add it to the list
                    foreach (System.Xml.XmlNode optionNode in CarrierNode.ChildNodes)
                    {
                        CarrierOption option = new CarrierOption();
                        option.Carrier = Carrier;
                        option.Percentage = decimal.Parse(optionNode.Attributes["Percentage"].Value);
                        string saction = optionNode.Attributes["Actions"].Value.ToString();
                        option.IsCustommer = Boolean.Parse(optionNode.Attributes["IsCustommer"].Value);
                        foreach (string actionName in saction.Split(','))
                        {
                            try
                            {
                                EventActions action = (EventActions)Enum.Parse(typeof(EventActions), actionName);
                                option.Actions = option.Actions | action;
                            }
                            catch
                            {

                            }
                        }
                         CarrierOptionList.Add(option);
                        this.CarrierOptions[Carrier] = CarrierOptionList;
                    }
                }
            }
        }

        protected System.Xml.XmlDocument ToXml()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            document.LoadXml(DefaultXml);

            System.Xml.XmlNode CarrierTemplate = document.DocumentElement.SelectSingleNode("Carrier");
            System.Xml.XmlNode optionTemplate = CarrierTemplate.SelectSingleNode("Option");
            document.DocumentElement.RemoveAll();

            foreach (CarrierAccount Carrier in this.CarrierOptions.Keys)
            {
                List<CarrierOption> CarrierOptions = this.CarrierOptions[Carrier];
                if (CarrierOptions.Count > 0)
                {
                    // Create the Carrier Node
                    System.Xml.XmlNode CarrierNode = CarrierTemplate.CloneNode(false);
                    CarrierNode.Attributes["CarrierAccountID"].Value = Carrier.CarrierAccountID;
                    document.DocumentElement.AppendChild(CarrierNode);

                    // Now add for each option an option node
                    CarrierOptions.Sort();
                    foreach (CarrierOption option in CarrierOptions)
                    {
                        // Create the Option Node
                        System.Xml.XmlNode optionNode = optionTemplate.CloneNode(false);
                        optionNode.Attributes["Percentage"].Value = option.Percentage.ToString();
                        optionNode.Attributes["Actions"].Value = Convert.ToString(option.Actions.ToString());
                        optionNode.Attributes["IsCustommer"].Value = Convert.ToString(option.IsCustommer);
                        CarrierNode.AppendChild(optionNode);
                    }
                }
            }

            return document;
        }

        public void Save()
        {
            SystemParameter.PostPaidCarrierOptions.LongTextValue = this.ToXml().InnerXml;
            Exception ex;
            ObjectAssembler.SaveOrUpdate(SystemParameter.PostPaidCarrierOptions, out ex);
        }

        /// <summary>
        /// Remove a Carrier option
        /// </summary>
        public void Remove(CarrierOption optionToRemove)
        {
            if (CarrierOptions.ContainsKey(optionToRemove.Carrier))
                CarrierOptions[optionToRemove.Carrier].Remove(optionToRemove);
        }

        /// <summary>
        /// Add a Carrier option
        /// </summary>
        /// <param name="optionToAdd"></param>
        public void Add(CarrierOption optionToAdd)
        {
            // If first option for this Carrier
            if (!CarrierOptions.ContainsKey(optionToAdd.Carrier))
            {
                CarrierOptions[optionToAdd.Carrier] = new List<CarrierOption>();
                CarrierOptions[optionToAdd.Carrier].Add(optionToAdd);
                return;
            }
            else
            {
                List<CarrierOption> options = CarrierOptions[optionToAdd.Carrier];
                if (!options.Contains(optionToAdd))
                {
                    options.Add(optionToAdd);
                    options.Sort();
                }
            }
        }

        public readonly static string DefaultXml = @"<?xml version=""1.0"" encoding=""utf-8""?><PostPaidCarrierOptions><Carrier CarrierAccountID=""""><Option Percentage="""" Actions="""" IsCustommer= """" /></Carrier></PostPaidCarrierOptions>";
    }
}
