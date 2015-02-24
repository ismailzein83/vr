  using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.MobileCDRAnalysis;
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

    }

    private void FillCombos()
    {
        Manager.BindCombo(ddlSources, Vanrise.Fzero.MobileCDRAnalysis.SwitchProfile.GetAll(), "Name", "Id", null, null);
    }

    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

      
        if (!IsPostBack)
        {
            SetPermissions();
            SetCaptions();
            FillCombos();
            ruImportedFile.TargetPhysicalFolder = path + ddlSources.SelectedItem.Text + "\\";
            gvImportedCalls.DataSource = new List<CDR>();
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

            Session["CDRs"] = null;

            foreach (UploadedFile f in ruImportedFile.UploadedFiles)
            {
                filePath = ruImportedFile.TargetPhysicalFolder + f.GetName();
                f.SaveAs(filePath, true);
                switch (f.GetExtension())
                {

                    case ".xls"://Excel (xls)
                    case ".xlsx"://Excel (xlsx)
                        Session["CDRs"] = CDR.GetDataFromExcel(filePath, ddlSources.SelectedValue.ToInt());
                        break;

                    case ".xml":
                        Session["CDRs"] = CDR.GetDataFromXml(filePath, ddlSources.SelectedValue.ToInt());
                        break;

                    default:
                        Session["CDRs"] = null;
                        break;

                }
            }

            if ( Session["CDRs"]==null)
            {
                Session["CDRs"] = new List<CDR>();
            }
                gvImportedCalls.DataSource = Session["CDRs"] ;
                gvImportedCalls.DataBind();

        }
        else
        {
            ShowError( "This file type is not allowed or no file uploaded");
        }

    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        if (ddlSources.SelectedValue.ToInt() != 0 && gvImportedCalls.Items.Count > 0)
        {
            CDR.Confirm(ddlSources.SelectedValue.ToInt(), (DataTable)Session["CDRs"], CurrentUser.User.ID);
            gvImportedCalls.DataSource = new List<CDR>();
            gvImportedCalls.DataBind();
            ShowAlert("CDRs have been saved to database");
        }
        else
        {
            ShowError("Import Items to grid before confirming");
        }

    }

    protected void btnImportClear_Click(object sender, EventArgs e)
    {
        gvImportedCalls.DataSource = new List<CDR>();
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