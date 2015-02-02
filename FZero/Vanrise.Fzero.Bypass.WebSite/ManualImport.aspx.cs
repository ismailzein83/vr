  using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;
using System.Data;
using System.Configuration;

public partial class ManualImports : BasePage
{
    #region Methods

    string path = ConfigurationManager.AppSettings["UploadPath"];

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ManualImport))
            PreviousPageRedirect();
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.ManualImport;
       

        int columnIndex = 0;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.Type;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.Reference;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.DurationsinSeconds;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.a_number;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.b_number;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CLI;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.OriginationNetwork;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.Carrier;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.AttemptDateTime;

    }

    private void FillCombos()
    {
        Manager.BindCombo(ddlSources, Vanrise.Fzero.Bypass.Source.GetAllSources(), "Name", "Id", null, null);
    }

    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (CurrentUser.portalType != 2)
            PreviousPageRedirect();


        if (!IsPostBack)
        {
            SetPermissions();
            SetCaptions();
            FillCombos();
            ruImportedFile.TargetPhysicalFolder = path + ddlSources.SelectedItem.Text + "\\";
            gvImportedCalls.DataSource = new List<GeneratedCall>();
            gvImportedCalls.DataBind();
        }
    }

    protected bool IsValidFile()
    {
        if (ruImportedFile.UploadedFiles.Count <= 0)
        {
            return false;
        }

        return true;
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {

        if (IsValidFile())
        {
            string filePath = string.Empty;

            Session["ImportedCalls"] = null;

            foreach (UploadedFile f in ruImportedFile.UploadedFiles)
            {
                filePath = ruImportedFile.TargetPhysicalFolder + f.GetName();
                f.SaveAs(filePath, true);
                switch (f.GetExtension())
                {

                    case ".xls"://Excel (xls)
                    case ".xlsx"://Excel (xlsx)
                        Session["ImportedCalls"] = GeneratedCall.GetDataFromExcel(filePath, ddlSources.SelectedValue.ToInt());
                        break;

                    case ".xml":
                        Session["ImportedCalls"] = GeneratedCall.GetDataFromXml(filePath, ddlSources.SelectedValue.ToInt());
                        break;

                    default:
                        Session["ImportedCalls"] = null;
                        break;

                }
            }

            if ( Session["ImportedCalls"]==null)
            {
                Session["ImportedCalls"] = new List<GeneratedCall>();
            }
                gvImportedCalls.DataSource = Session["ImportedCalls"] ;
                gvImportedCalls.DataBind();

        }
        else
        {
            ShowError("This file type is not allowed or no file uploaded");
        }

    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        if (ddlSources.SelectedValue.ToInt() != 0 && gvImportedCalls.Items.Count > 0)
        {
            GeneratedCall.Confirm(ddlSources.SelectedValue.ToInt(), (DataTable)Session["ImportedCalls"], CurrentUser.ApplicationUserID);
            gvImportedCalls.DataSource = new List<GeneratedCall>();
            gvImportedCalls.DataBind();
            ShowAlert("Generated Calls have been saved to database");
            LoggedAction.AddLoggedAction((int)Enums.ActionTypes.Importedgeneratedcalls, CurrentUser.User.ID);
        }
        else
        {
            ShowError("Import Items to grid before confirming");
        }
      

    }

    protected void btnImportClear_Click(object sender, EventArgs e)
    {
        gvImportedCalls.DataSource = new List<GeneratedCall>();
        gvImportedCalls.DataBind();
        
        ddlSources.SelectedIndex = 0;
        ruImportedFile.UploadedFiles.Clear();
    }

    protected void ddlSources_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        ruImportedFile.TargetPhysicalFolder = path + ddlSources.SelectedItem.Text +"\\";
    }

    #endregion
}