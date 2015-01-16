using System.Xml;

namespace TABS.SpecialSystemParameters
{
    public class SimpleParameterXml : System.Xml.XmlDocument
    {
        public SimpleParameterXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                xml = SystemParameter.DefaultXml;
            base.LoadXml(xml);
        }

        public virtual void Set(string property, string value)
        {
            XmlNode node = this.DocumentElement.SelectSingleNode(property);
            if (node == null)
            {
                XmlElement element = this.CreateElement(property);
                XmlCDataSection cdata = this.CreateCDataSection(value);
                DocumentElement.AppendChild(element);
                element.AppendChild(cdata);
            }
            else node.FirstChild.Value = value;
        }

        public virtual string Get(string property, string defaultIfNullOrEmpty)
        {
            string value = Get(property);
            return (string.IsNullOrEmpty(value)) ? defaultIfNullOrEmpty : value; 
        }

        public virtual string Get(string property)
        {
            XmlNode node = DocumentElement.SelectSingleNode(property);
            if (node == null) return null;
            else return node.FirstChild.Value;
        }
    }
}
