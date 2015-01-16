using System;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace TABS.Addons.WebControls
{
    /// <summary>
    /// A drop-down list to select a carrier group
    /// </summary>
    public class CarrierGroupSelector : System.Web.UI.WebControls.DropDownList
    {
        public virtual bool IncludeEmptyItem { get; set; }

        public CarrierGroupSelector()
        {

        }

        protected override void OnInit(EventArgs e)
        {
            Reload();
            base.OnInit(e);
        }

        const char replacement = '\u00B7';

        public virtual void Reload()
        {
            string selectedValue = this.SelectedValue;
            this.Items.Clear();

            if (IncludeEmptyItem)
                this.Items.Add("");

            var groups = ObjectAssembler.CurrentSession.CreateQuery(
                "FROM CarrierGroup g ORDER BY Path"
                ).List<TABS.CarrierGroup>();

            foreach (var group in groups)
            {

                if (this.Type == CarrierGroupType.Customers)
                {
                    bool add = true;
                    foreach (var carrier in group.Accounts)
                    {
                        if (carrier.AccountType == AccountType.Termination)
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        string stars = new string(replacement, (group.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length - 1) * 4);
                        this.Items.Add(new ListItem(stars + " " + group.CarrierGroupName, group.CarrierGroupID.ToString()));
                    }
                }
                else if (this.Type == CarrierGroupType.Suppliers)
                {
                    bool add = true;
                    foreach (var carrier in group.Accounts)
                    {
                        if (carrier.AccountType == AccountType.Client)
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        string stars = new string(replacement, (group.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length - 1) * 4);
                        this.Items.Add(new ListItem(stars + " " + group.CarrierGroupName, group.CarrierGroupID.ToString()));
                    }
                }
                else
                {
                    string stars = new string(replacement, (group.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length - 1) * 4);
                    this.Items.Add(new ListItem(stars + " " + group.CarrierGroupName, group.CarrierGroupID.ToString()));

                }
            }

            if (selectedValue != null)
                if (this.Items.FindByValue(selectedValue) != null)
                    this.SelectedValue = selectedValue;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter stringWriter = new System.IO.StringWriter(sb);
            Html32TextWriter theWriter = new Html32TextWriter(stringWriter);
            base.Render(theWriter);
            sb.Replace("&#183;", "&nbsp;");
            writer.Write(sb);
        }

        public virtual CarrierGroupType Type
        {
            get;
            set;
        }

        public enum CarrierGroupType
        {
            Both,
            Customers,
            Suppliers

        }
    }
}