  using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;
using System.Data;
using System.Configuration;

public partial class RelatedNumbers : BasePage
{
    #region Methods

    
    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.RelatedNumbers))
            PreviousPageRedirect();
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.RelatedNumbers;

        int columnIndex = 0;
        gvImportedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.RelatedNumbers;

    }

    private void FillCombos()
    {
        List<MobileOperator> listMobileOperator = Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperatorsList();

        ddlMobileOperators.Items.Clear();
        ddlMobileOperators.Items.Add(new RadComboBoxItem(Resources.Resources.PleaseSelect, "0"));
        foreach (MobileOperator i in listMobileOperator)
        {
            ddlMobileOperators.Items.Add(new RadComboBoxItem(i.User.FullName, i.ID.ToString()));
        }

        Manager.BindCombo(ddlReport, Vanrise.Fzero.Bypass.Report.GetAllReports(), "ReportID", "Id", Resources.Resources.PleaseSelect, "0");
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
            ruImportedFile.TargetPhysicalFolder = ConfigurationManager.AppSettings["RelatedNumbers"];
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

            Session["RelatedNumbers"] = null;

            foreach (UploadedFile f in ruImportedFile.UploadedFiles)
            {
                filePath = ruImportedFile.TargetPhysicalFolder + f.GetName();
                f.SaveAs(filePath, true);
                switch (f.GetExtension())
                {

                    case ".xls"://Excel (xls)
                    case ".xlsx"://Excel (xlsx)
                        Session["RelatedNumbers"] = RelatedNumberMapping.GetDataFromExcel(filePath, ddlMobileOperators.SelectedValue.ToInt());
                        break;

                    case ".xml":
                        Session["RelatedNumbers"] = RelatedNumberMapping.GetDataFromXml(filePath, ddlMobileOperators.SelectedValue.ToInt());
                        break;

                    default:
                        Session["RelatedNumbers"] = null;
                        break;

                }
            }

            if ( Session["RelatedNumbers"]==null)
            {
                Session["RelatedNumbers"] = new List<GeneratedCall>();
            }
                gvImportedCalls.DataSource = Session["RelatedNumbers"] ;
                gvImportedCalls.DataBind();

        }
        else
        {
            ShowError("This file type is not allowed or no file uploaded");
        }

    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        RelatedNumberMapping.Confirm((DataTable)Session["RelatedNumbers"], CurrentUser.ApplicationUserID, ddlReport.SelectedValue.ToInt());
        gvImportedCalls.DataSource = new List<GeneratedCall>();
        gvImportedCalls.DataBind();
        ShowAlert("Related Numbers have been saved to database");
        LoggedAction.AddLoggedAction((int)Enums.ActionTypes.AddedRelatedNumbers, CurrentUser.User.ID);

    }

    protected void btnImportClear_Click(object sender, EventArgs e)
    {
        gvImportedCalls.DataSource = new List<GeneratedCall>();
        gvImportedCalls.DataBind();
        
        ddlMobileOperators.SelectedIndex = 0;
        ruImportedFile.UploadedFiles.Clear();
    }

  
    #endregion
}