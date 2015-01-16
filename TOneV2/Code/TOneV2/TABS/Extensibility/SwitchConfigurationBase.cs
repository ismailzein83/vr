using System.Collections.Generic;

namespace TABS.Extensibility
{
    public class SwitchConfigurationBase : System.Xml.XmlDocument
    {
        public static System.Text.RegularExpressions.Regex CdrInOutParser = new System.Text.RegularExpressions.Regex(
            @"(?<Cdr>[^;,:]+):([^;]+,)*CDR(=(?<Prefix>[#0-9]+)){0,1}($|((,[^;]+);)*)",
            System.Text.RegularExpressions.RegexOptions.Compiled
            | System.Text.RegularExpressions.RegexOptions.ExplicitCapture
            | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        public static System.Text.RegularExpressions.Regex RoutingInOutParser = new System.Text.RegularExpressions.Regex(
            @"(?<Route>[^;,:]+):([^;]+,)*RT(=(?<Prefix>[#0-9]+)){0,1}($|((,[^;]+);)*)",
            System.Text.RegularExpressions.RegexOptions.Compiled
            | System.Text.RegularExpressions.RegexOptions.ExplicitCapture
            | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        protected Switch _Switch;

        public string GetAttribute(System.Xml.XmlNode node, string attributeName)
        {
            return node.Attributes[attributeName] != null ? node.Attributes[attributeName].Value : string.Empty;
        }

        public Switch Switch
        {
            get { return _Switch; }
            set { _Switch = value; }
        }

        public SwitchConfigurationBase(Switch ownerSwitch, string XML)
        {
            this._Switch = ownerSwitch;
            this.InnerXml = XML;
            InitializeDefaultParameters();
        }

        protected SwitchConfigurationBase()
        {

        }

        public virtual void InitializeDefaultParameters() 
        { 
            
        }

        public virtual void Reload() {}

        public virtual string SwitchManagerName { get { return GetAttribute("SwitchManager"); } set { SetAttribute("SwitchManager", value); } }

        #region Configuration and Parameters

        protected string GetAttribute(string name) { return DocumentElement.Attributes[name].Value; }
        protected void SetAttribute(string name, string value) { DocumentElement.Attributes[name].Value = value; }

        /// <summary>
        /// Gets the parameters node
        /// </summary>
        protected System.Xml.XmlNode ParametersNode { get { return DocumentElement.SelectSingleNode("Parameters"); } }

        protected System.Xml.XmlNode GetParameter(string name, bool create)
        {
            System.Xml.XmlNode parameter = ParametersNode.SelectSingleNode("Parameter[@Name='" + name + "']");
            if (create && parameter == null)
            {
                parameter = GetParameter("NullParameter", false).CloneNode(true);
                parameter.Attributes["Name"].Value = name;
                ParametersNode.AppendChild(parameter);
            }
            return parameter;
        }

        protected void RemoveParameter(string name)
        {
            System.Xml.XmlNode parameter = GetParameter(name, false);
            if (parameter != null)
                ParametersNode.RemoveChild(parameter);
        }

        protected string GetParameterValue(string name)
        {
            System.Xml.XmlNode parameter = GetParameter(name, false);
            if (parameter == null) return null;
            else
                return parameter.FirstChild.Value;
        }

        protected System.Xml.XmlNode SetParameterValue(string name, string value)
        {
            System.Xml.XmlNode parameter = GetParameter(name, true);
            parameter.FirstChild.Value = value;
            return parameter;
        }

        protected bool IsNullParameter(System.Xml.XmlNode paramNode)
        {
            return (paramNode.Name == "Parameter" && paramNode.Attributes["Name"].Value == "NullParameter");
        }

        public void ClearParameters()
        {
            List<System.Xml.XmlNode> paramList = new List<System.Xml.XmlNode>();
            foreach (System.Xml.XmlNode node in ParametersNode.ChildNodes)
                if (!IsNullParameter(node))
                    paramList.Add(node);
            foreach (System.Xml.XmlNode node in paramList)
                ParametersNode.RemoveChild(node);
        }

        #endregion Configuration and Parameters

        /// <summary>
        /// The Configuration XML Template Definition.
        /// </summary>
        protected static readonly string _ConfigurationXmlTemplate =
            @"<?xml version=""1.0"" encoding=""utf-8"" ?>
              <SwitchConfiguration SwitchManager="""">
                <Parameters>
                  <Parameter Name=""NullParameter""><![CDATA[]]></Parameter>
                </Parameters>
              </SwitchConfiguration>";

        /// <summary>
        /// Get the Default Configration
        /// </summary>
        /// <returns></returns>
        public static SwitchConfigurationBase GetDefaultConfiguration()
        {
            SwitchConfigurationBase defaultConfig = new SwitchConfigurationBase();
            defaultConfig.LoadXml(_ConfigurationXmlTemplate);
            return defaultConfig;
        }
    }
}