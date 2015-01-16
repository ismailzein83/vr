using System.Collections.Generic;
using System.Xml;

namespace TABS.NOC
{
    public class EscalationLevel
    {
        System.Xml.XmlNode _XmlNode;

        public XmlNode XmlNode { get { return _XmlNode; } }
        public XmlDocument OwnerDocument { get { return _XmlNode.OwnerDocument; } }

        public virtual string Get(string property)
        {
            XmlNode node = _XmlNode.SelectSingleNode(property);
            if (node == null) return null;
            else return node.FirstChild.Value;
        }

        public virtual void Set(string property, string value)
        {
            XmlNode node = _XmlNode.SelectSingleNode(property);
            if (node == null)
            {
                XmlElement element = _XmlNode.OwnerDocument.CreateElement(property);
                XmlCDataSection cdata = _XmlNode.OwnerDocument.CreateCDataSection(value);
                _XmlNode.AppendChild(element);
                element.AppendChild(cdata);
            }
            else node.FirstChild.Value = value;
        }

        public int CarrierProfileID { get { return int.Parse(Get("CarrierProfileID")); } set { Set("CarrierProfileID", value.ToString()); } }
        public string Name { get { return Get("Name"); } set { Set("Name", value); } }
        public string PhoneNbr { get { return Get("PhoneNbr"); } set { Set("PhoneNbr", value); } }
        public string Email { get { return Get("Email"); } set { Set("Email", value); } }
        public string IM { get { return Get("IM"); } set { Set("IM", value); } }

        public EscalationLevel(XmlNode xml)
        {
            this._XmlNode = xml;
        }

        public EscalationLevel(XmlDocument xmlDoc)
        {
            XmlElement element = xmlDoc.CreateElement("Level");
            _XmlNode = xmlDoc.DocumentElement.AppendChild(element);
        }

        /// <summary>
        /// Returns the list of available Levels
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static List<EscalationLevel> GetList(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            List<EscalationLevel> details = new List<EscalationLevel>();
            foreach (XmlNode node in xmlDoc.DocumentElement.SelectNodes("Level"))
                details.Add(new EscalationLevel(node));
            return details;
        }

        /// <summary>
        /// Create a new level
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EscalationLevel Create(string xml, out List<EscalationLevel> detailsList)
        {
            XmlDocument xmlDoc = new XmlDocument();

            // Empty 
            if (string.IsNullOrEmpty(xml))
                xml = _DefaultXml;

            xmlDoc.LoadXml(xml);

            EscalationLevel escalationLevel = new EscalationLevel(xmlDoc);
            xml = escalationLevel.OwnerDocument.InnerXml;
            detailsList = GetList(xml);

            return detailsList[detailsList.Count - 1];
        }

        /// <summary>
        /// Remove the level at the given index (if they exist)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="index"></param>
        public static void Remove(string xml, int index)
        {
            List<EscalationLevel> details = GetList(xml);
            if (index >= 0 && index < details.Count)
            {
                XmlNode node = details[index].XmlNode;
                XmlDocument xmlDoc = details[index].OwnerDocument;
                xmlDoc.DocumentElement.RemoveChild(node);
                details.RemoveAt(index);
            }
        }

        public static string _DefaultXml
        {
            get { return @"<?xml version=""1.0"" encoding=""utf-8"" ?><NOCEscaltionLevel></NOCEscaltionLevel>"; }
        }

    }
}
