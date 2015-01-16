using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace TABS.Addons.WebControls
{
    public abstract class SwitchConfigurationControl : System.Web.UI.WebControls.Table, System.Web.UI.INamingContainer
    {
        protected Extensibility.SwitchConfigurationBase _SwitchConfiguration;

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        /// <summary>
        /// Gets or Sets the Switch this Configuration Control is bound to.
        /// When the switch is set, the Initialize function is called.
        /// </summary>
        public virtual Extensibility.SwitchConfigurationBase SwitchConfiguration
        {
            get { return _SwitchConfiguration; }
            set { if (_SwitchConfiguration != value) { _SwitchConfiguration = value; InitializeFromConfiguration(); } }
        }

        /// <summary>
        /// Called when a Switch Object is set, so that the control initializes itself
        /// (Create the controls and so on)
        /// </summary>
        protected abstract void InitializeFromConfiguration();

        /// <summary>
        /// Update the Switch Configuration XML from the state of the controls 
        /// (Should be triggered by Consuming page / control when user clicks on "Save" or "Update" or so...)
        /// </summary>
        public abstract void UpdateSwitchConfiguration();

        #region Configuration and Parameters

        /// <summary>
        /// Gets or Sets the Parameter Value, given the parameter name 
        /// </summary>
        /// <param name="name">Parameter Name</param>
        public virtual string this[string name] { get { return GetParameterValue(name); } set { SetParameterValue(name, value); } }

        protected string GetAttribute(string name) { return SwitchConfiguration.DocumentElement.Attributes[name].Value; }
        protected void SetAttribute(string name, string value) { SwitchConfiguration.DocumentElement.Attributes[name].Value = value; }

        /// <summary>
        /// Gets the parameters node
        /// </summary>
        protected System.Xml.XmlNode ParametersNode { get { return SwitchConfiguration.DocumentElement.SelectSingleNode("Parameters"); } }

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
        /// Gets or Sets the switch manager name from the switch configuration
        /// </summary>
        public string SwitchManagerName { get { return GetAttribute("SwitchManager"); } set { SetAttribute("SwitchManager", value); } }

        protected Dictionary<string, TextBox> KnownParamsTextBoxes = new Dictionary<string, TextBox>();

        private TableRow CreateGroupSplitter()
        {
            TableRow row = new TableRow();
            TableHeaderCell groupSplitCell = new TableHeaderCell();
            groupSplitCell.CssClass = "GroupSplit";
            groupSplitCell.ColumnSpan = 5;
            groupSplitCell.Text = "<hr/>";
            row.Cells.Add(groupSplitCell);
            this.Rows.Add(row);
            return row;
        }

        protected TextBox AddKnownParameter(string header, int rowSpan, string name)
        {
            int index = 0;

            TableRow tableRow = new TableRow();

            if (header != null)
            {
                tableRow.Cells.Add(new TableHeaderCell());
                tableRow.Cells[index].Text = header;
                tableRow.Cells[index].RowSpan = rowSpan;
                index++;
            }

            tableRow.Cells.Add(new TableHeaderCell());
            tableRow.Cells[index++].Text = name.Replace("_", " ");

            TextBox textBox = new TextBox();
            textBox.Columns = 100;
            textBox.ID = name;
            tableRow.Cells.Add(new TableCell());
            tableRow.Cells[index++].Controls.Add(textBox);

            KnownParamsTextBoxes[name] = textBox;

            // Add the row...
            this.Rows.Add(tableRow);

            return textBox;
        }
        protected TextBox AddKnownParameterTextArea(string header, int rowSpan, string name)
        {
            int index = 0;

            TableRow tableRow = new TableRow();

            if (header != null)
            {
                tableRow.Cells.Add(new TableHeaderCell());
                tableRow.Cells[index].Text = header;
                tableRow.Cells[index].RowSpan = rowSpan;
                index++;
            }

            tableRow.Cells.Add(new TableHeaderCell());
            tableRow.Cells[index++].Text = name.Replace("_", " ");

            TextBox textBox = new TextBox();
            textBox.Columns = 100;
            textBox.TextMode = TextBoxMode.MultiLine;
            textBox.Height = 300;
            textBox.ID = name;
            tableRow.Cells.Add(new TableCell());
            tableRow.Cells[index++].Controls.Add(textBox);

            KnownParamsTextBoxes[name] = textBox;

            // Add the row...
            this.Rows.Add(tableRow);

            return textBox;
        }

        protected TextBox AddKnownParameterTextArea(string header, int rowSpan, string name,int hight,int width)
        {
            int index = 0;

            TableRow tableRow = new TableRow();

            if (header != null)
            {
                tableRow.Cells.Add(new TableHeaderCell());
                tableRow.Cells[index].Text = header;
                tableRow.Cells[index].RowSpan = rowSpan;
                index++;
            }

            tableRow.Cells.Add(new TableHeaderCell());
            tableRow.Cells[index++].Text = name.Replace("_", " ");

            TextBox textBox = new TextBox();
            textBox.Columns = width;
            textBox.TextMode = TextBoxMode.MultiLine;
            textBox.Height = Height;
            textBox.ID = name;
            tableRow.Cells.Add(new TableCell());
            tableRow.Cells[index++].Controls.Add(textBox);

            KnownParamsTextBoxes[name] = textBox;

            // Add the row...
            this.Rows.Add(tableRow);

            return textBox;
        }

        /// <summary>
        /// Create a row for a check box control and return the created control
        /// </summary>
        /// <param name="name">The name (title) of the control</param>
        /// <returns></returns>
        protected CheckBox AddCheckBoxParameter(string name, string ID)
        {
            return AddCheckBoxParameter(null, 0, name, ID);
        }
        protected CheckBox AddCheckBoxParameter(string header, int rowSpan, string name, string ID)
        {
            TableRow tableRow = new TableRow();
            int index = 0;

            if (header != null)
            {
                tableRow.Cells.Add(new TableHeaderCell());
                tableRow.Cells[index].Text = header;
                tableRow.Cells[index].RowSpan = rowSpan;
                index++;
            }


            tableRow.Cells.Add(new TableHeaderCell());
            tableRow.Cells[index].ColumnSpan = 2;
            tableRow.Cells[index].Style.Add("text-align", "left");
            CheckBox checkBox = new CheckBox();
            checkBox.Text = name;
            checkBox.ID = ID;
            tableRow.Cells[index].Controls.Add(checkBox);
            // Add the row...
            this.Rows.Add(tableRow);
            return checkBox;
        }

        protected Button AddPopupButton(string name, string ID, string popupUrl, string popupTitle)
        {
            TableRow tableRow = new TableRow();
            tableRow.Cells.Add(new TableHeaderCell());
            tableRow.Cells[0].ColumnSpan = 2;
            tableRow.Cells[0].Style.Add("text-align", "left");
            Button button = new Button();
            button.Text = name;
            button.ID = ID;
            button.OnClientClick = string.Format("temp = window.open('{0}', '{1}', 'height=400,width=800'); return false;", popupUrl, popupTitle);
            tableRow.Cells[0].Controls.Add(button);
            // Add the row...
            this.Rows.Add(tableRow);
            return button;
        }

        /// <summary>
        /// Create a row for a drop down list control and return the created control
        /// </summary>
        /// <param name="name">The name (title) of the control</param>
        /// <returns></returns>
        protected DropDownList AddDropDownListParameter(string name, string ID, IEnumerable options)
        {
            TableRow tableRow = new TableRow();

            tableRow.Cells.Add(new TableHeaderCell());
            tableRow.Cells[0].Text = name;

            DropDownList ddl = new DropDownList();
            foreach (object option in options)
                ddl.Items.Add(new ListItem(option.ToString()));
            ddl.ID = ID;
            tableRow.Cells.Add(new TableCell());
            tableRow.Cells[1].Controls.Add(ddl);
            // Add the row...
            this.Rows.Add(tableRow);
            return ddl;
        }

        protected DropDownList AddDropDownListParameter(string name, string ID, IEnumerable options, string textField, string valueField)
        {
            TableRow tableRow = new TableRow();

            tableRow.Cells.Add(new TableHeaderCell());
            tableRow.Cells[0].Text = name;

            DropDownList ddl = new DropDownList();
            ddl.ID = ID;
            ddl.DataSource = options;
            ddl.DataValueField = valueField;
            ddl.DataTextField = textField;
            tableRow.Cells.Add(new TableCell());
            tableRow.Cells[1].Controls.Add(ddl);
            // Add the row...
            this.Rows.Add(tableRow);
            return ddl;
        }

        protected LinkButton AddLinkButtonParameter(string header, int rowSpan, string name, string ID, string url, string title)
        {
            TableRow tableRow = new TableRow();
            int index = 0;

            if (header != null)
            {
                tableRow.Cells.Add(new TableHeaderCell());
                tableRow.Cells[index].Text = header;
                tableRow.Cells[index].RowSpan = rowSpan;
                index++;
            }


            tableRow.Cells.Add(new TableHeaderCell());
            tableRow.Cells[index].ColumnSpan = 1;
            tableRow.Cells[index].Style.Add("text-align", "right");
            tableRow.Cells[index].Height = 20;
            LinkButton btn = new LinkButton();
            btn.Text = name;
            btn.ID = ID;
            btn.Font.Bold = true;
            btn.OnClientClick = string.Format("temp = window.open('{0}', '{1}','menubar=0,status=0,toolbar=0,location=0,resizable=1,scrollbars=1,height=400,width=800'); return false;", url, title);
            tableRow.Cells[index].Controls.Add(btn);
            // Add the row...
            this.Rows.Add(tableRow);
            return btn;
        }


    }
}