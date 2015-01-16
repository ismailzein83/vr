using System;
using System.Collections.Generic;
using System.Xml;


namespace TABS.SpecialSystemParameters
{
    public class BaseXmlDetails
    {
        protected System.Xml.XmlNode _XmlNode;

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

        protected BaseXmlDetails() { }

        public BaseXmlDetails(XmlNode xml) { this._XmlNode = xml; }

        public BaseXmlDetails(XmlDocument xmlDoc) 
        {
            CreateInXml(xmlDoc);
        }

        protected void CreateInXml(XmlDocument xmlDoc)
        {
            XmlElement element = xmlDoc.CreateElement(this.GetType().Name);
            _XmlNode = xmlDoc.DocumentElement.AppendChild(element);
        }

        /// <summary>
        /// Returns the list of available details from the system parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static List<T> Get<T>(SystemParameter parameter) where T : BaseXmlDetails, new()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(parameter.LongTextValue);
            List<T> details = new List<T>();
            foreach (XmlNode node in xmlDoc.DocumentElement.SelectNodes(typeof(T).Name))
            {
                T detail = new T();
                detail._XmlNode = node;
                details.Add(detail);
            }
            return details;
        }

        /// <summary>
        /// Create a new Mail Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected static T Create<T>(SystemParameter parameter, out List<T> detailsList, string defaultXml) where T : BaseXmlDetails, new()
        {
            XmlDocument xmlDoc = new XmlDocument();

            // Empty Parameter?
            if (string.IsNullOrEmpty(parameter.LongTextValue))
                parameter.LongTextValue = defaultXml;

            xmlDoc.LoadXml(parameter.LongTextValue);

            T detail = new T();
            detail.CreateInXml(xmlDoc);
            
            parameter.LongTextValue = detail.OwnerDocument.InnerXml;
            detailsList = Get<T>(parameter);

            return detailsList[detailsList.Count - 1];
        }

        /// <summary>
        /// Save the System Parameter Mail Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="details"></param>
        protected static Exception Save<T>(SystemParameter parameter, List<T> details, string defaultXml) where T : BaseXmlDetails
        {
            if (details.Count > 0)
            {
                XmlDocument xmlDoc = details[0].OwnerDocument;
                parameter.LongTextValue = xmlDoc.InnerXml;
            }
            else
            {
                parameter.LongTextValue = defaultXml;
            }

            Exception ex;
            ObjectAssembler.SaveOrUpdate(parameter, out ex);
            return ex;
        }

        /// <summary>
        /// Remove the details at the given index (if they exist)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="index"></param>
        protected static Exception Remove<T>(SystemParameter parameter, int index, string defaultXML) where T : BaseXmlDetails, new()
        {
            Exception ex = null;
            List<T> details = Get<T>(parameter);
            if (index >= 0 && index < details.Count)
            {
                XmlNode node = details[index].XmlNode;
                XmlDocument xmlDoc = details[index].OwnerDocument;
                xmlDoc.DocumentElement.RemoveChild(node);
                details.RemoveAt(index);
                ex = Save(parameter, details, defaultXML);
            }
            return ex;
        }
    }
}
