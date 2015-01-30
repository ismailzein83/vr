using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;

public partial class ManageCarriers : BasePage
{   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        divSuccess.Visible = false;
        divError.Visible = false;

        if (!IsPostBack)
        {
            GetData();
        }
    }

    #region Methods
    private void GetData()
    {
        List<Carrier> Carriers = CarrierRepository.GetCarriers(Current.User.Id).OrderByDescending(l => l.Id).ToList();
        Session["Carriers"] = Carriers;
        rptCarriers.DataSource = Carriers;
        rptCarriers.DataBind();
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        LinkButton lk = (LinkButton)sender;
        int id = 0;
        int.TryParse(lk.CommandArgument.ToString(), out id);
        if (CarrierRepository.Delete(id))
        {
            ActionLog action = new ActionLog();
            action.ObjectId = id;
            action.ObjectType = "Carrier";
            //action.Description = Utilities.SerializeLINQtoXML<LoanProduct>(contractbidd);
            action.ActionType = (int)Enums.ActionType.Delete;
            AuditRepository.Save(action);
            GetData();
        }
        else
        {
            JavaScriptAlert("We can't delete a record with child rows");
            GetData();
            return;
        }
    }
    #endregion
    private void SetError(string Error)
    {
        divSuccess.Visible = false;
        divError.Visible = true;
        lblError.Text = Error;
    }
    private void SetSuccess(string Message)
    {
        divSuccess.Visible = true;
        divError.Visible = false;
        lblError.Text = Message;
    }
    protected void btnImport_Click(object sender, EventArgs e)
    {
        String filePath = String.Empty;

        if ((fileAttachment.PostedFile != null) && (fileAttachment.PostedFile.ContentLength > 0))
        {
            string fn = System.IO.Path.GetFileName(fileAttachment.PostedFile.FileName);
            string extension = System.IO.Path.GetExtension(fileAttachment.PostedFile.FileName);
            if (extension == ".csv")
            {
                filePath = Server.MapPath("Resources\\") + DateTime.Now.ToString("yyyymmddhhmmss") + fn;
                try
                {
                    fileAttachment.PostedFile.SaveAs(filePath);
                }
                catch (System.Exception ex)
                {
                    SetError("Error while saving the file, please retry");
                    return;
                }

                List<string> numbers = new List<string>();
                List<Carrier> LstCarriers = new List<Carrier>();

                char delimiter = Convert.ToChar(",".Replace("CR", "\r").Replace("LF", "\n").Replace("TAB", "\t"));
                LstCarriers = GetFromCSVFile(filePath, true, delimiter);
                if (LstCarriers == null)
                {
                    SetError("There is an error in the file format");
                    return;
                }
                foreach (Carrier cc in LstCarriers)
                    CarrierRepository.Save(cc);
                SetSuccess("Carriers added succefully");
                GetData();
            }
            else
            {
                SetError("File should be saved as csv file");
                //JavaScriptAlert("File should be saved as csv file");
            }
        }
    }

    public static List<Carrier> GetFromCSVFile(string filePath, bool skipFirstLine, char delimiter)
    {
        List<string> numbers = new List<string>();
        List<string> numb = new List<string>();
        List<Carrier> Carriers = new List<Carrier>();

        try
        {
            StreamReader reader = new StreamReader(filePath);
            string data = reader.ReadToEnd();
            reader.Close();

            if (skipFirstLine)
            {
                int index = data.IndexOf("\r");
                if (index > 0)
                    data = data.Remove(0, index);
            }
            string d = "\r\n";
            data = data.Replace(d, ";");


            //string[] entities = data.Split(new char[] { delimiter });
            string[] entities = data.Split(';');

            foreach (string entity in entities)
            {
                if (entity != "")
                {
                    string s = "";
                    s = entity;

                    numbers.Add(s);
                }
            }


            foreach (string en in numbers)
            {
                string[] ents = en.Split(',');
                Carrier c = new Carrier();
                c.CustomerId = Current.User.Id;
                c.Name = ents[0];
                c.Prefix = ents[1];
                c.ShortName = ents[2];
                Carriers.Add(c);
            }

            return Carriers;
        }
        catch
        {
            return null;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtPrefix.Text) || String.IsNullOrEmpty(txtShortName.Text))
        {
            return;
        }

        string ShortName = txtShortName.Text.Replace("!", "");
        ShortName = ShortName.Replace("$", "");
        ShortName = ShortName.Replace("~", "");

        if (CarrierRepository.ExistShortName(ShortName))
        {
            lblError.Text = "Short Name Exist";
            return;
        }

        Carrier newCarrier = new Carrier();

        ActionLog action = new ActionLog();
        action.ObjectId = Current.User.Id;
        action.ObjectType = "Carrier";

        if (String.IsNullOrEmpty(HdnId.Value))
        {
            newCarrier.CustomerId = Current.User.Id;
            action.ActionType = (int)Enums.ActionType.Add;
        }
        else
        { 
            int id = 0;
            if (Int32.TryParse(HdnId.Value, out id))
            {
                newCarrier = CarrierRepository.Load(id);
                if (newCarrier == null) return;
                action.ActionType = (int)Enums.ActionType.Modify;
            }
            else
            {
                return;
            }
        }

        newCarrier.Name = txtName.Text;
        newCarrier.Prefix = txtPrefix.Text;
        newCarrier.ShortName = ShortName;
        CarrierRepository.Save(newCarrier);

        action.Description = Utilities.SerializeLINQtoXML<Carrier>(newCarrier);
        
        AuditRepository.Save(action);

        GetData();
    }
}