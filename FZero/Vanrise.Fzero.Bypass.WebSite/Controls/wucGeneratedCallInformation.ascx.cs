using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;

public partial class wucGeneratedCallInformation : System.Web.UI.UserControl
{

    #region Properties
   

    public string GeneratedCallId
    {
        get
        {
            return currentObject.ID.ToString();
        }
        set
        {
            currentObject.ID = value.ToInt();
        }
    }

    public prVwGeneratedCall_Result currentObject
    {
        get
        {
            if (Session["wucGeneratedCallInformation.currentObject"] is prVwGeneratedCall_Result)
                return (prVwGeneratedCall_Result)Session["wucGeneratedCallInformation.currentObject"];
            return new prVwGeneratedCall_Result();
        }
        set
        {
            Session["wucGeneratedCallInformation.currentObject"] = value;
        }
    }

    
    #endregion

    #region Methods

    public void FillData(prVwGeneratedCall_Result prVwGeneratedCall_Result)
    {
        hdnId.Value = prVwGeneratedCall_Result.ID.ToString();
        currentObject = prVwGeneratedCall_Result;
        txtCaseID.Text = currentObject.CaseID;
        txta_number.Text = currentObject.a_number;
        txtb_number.Text = currentObject.b_number;
        txtCLI.Text = currentObject.CLI;
        txtFeedbackNotes.Text = currentObject.FeedbackNotes;

        txtOriginationNetwork.Text = currentObject.OriginationNetwork;

        txtReportRealID.Text = currentObject.ReportRealID;


        txtMobileOperator.Text = currentObject.MobileOperatorName;
        txtMobileOperatorFeedback.Text = currentObject.MobileOperatorFeedbackName;


        txtGeneratedBy.Text = currentObject.SourceName;
        txtReceivedBy.Text = currentObject.ReceivedSourceName;
        txtStatus.Text = currentObject.StatusName;

        if (txtStatus.Text == "Fraud")
        {
            txtStatus.ForeColor = System.Drawing.Color.Red;
        }
        else if (txtStatus.Text == "Suspect")
        {
            txtStatus.ForeColor = System.Drawing.Color.OrangeRed;
        }
        else if (txtStatus.Text == "Pending")
        {
            txtStatus.ForeColor = System.Drawing.Color.Gray;
        }
        else if (txtStatus.Text == "Clean")
        {
            txtStatus.ForeColor = System.Drawing.Color.Green;
        }
        else if (txtStatus.Text == "Null")
        {
            txtStatus.ForeColor = System.Drawing.Color.Orange;
        }
        else if (txtStatus.Text == "Ignored")
        {
            txtStatus.Font.Strikeout = true;
        }



        txtPriority.Text = currentObject.PriorityName;
        txtAssignedToFullName.Text = currentObject.AssignedToFullName;
        txtReportingStatus.Text = currentObject.ReportingStatusName;
        txtAttemptDateTime.Text = currentObject.AttemptDateTime.ToString();
        txtFeedbackDateTime.Text = currentObject.FeedbackDateTime.ToString();
        txtLevelOneComparisonDateTime.Text = currentObject.LevelOneComparisonDateTime.ToString();
        txtLevelTwoComparisonDateTime.Text = currentObject.LevelTwoComparisonDateTime.ToString();
        if (currentObject.ToneFeedbackID != null)
        {
            switch (currentObject.ToneFeedbackID)
            {
                case (int) Enums.ToneFeedback.Found:
                    txtTOnefeedback.Text = "Found";
                    break;

                case (int)Enums.ToneFeedback.NotFound:
                    txtTOnefeedback.Text = "Not Found";
                    break;

                case (int)Enums.ToneFeedback.DatanotAvailable:
                    txtTOnefeedback.Text = "Data not Available";
                    break;
            }
        }
        txtAssignmentDateTime.Text = currentObject.AssignmentDateTime.ToString();

        int columnIndex = 0;
        gvHistory.Columns[columnIndex++].HeaderText = Resources.Resources.ChangeDone;
        gvHistory.Columns[columnIndex++].HeaderText = Resources.Resources.ChangeOn;
        gvHistory.DataSource = CasesLog.GetCaseLog(hdnId.Value.ToInt());
        gvHistory.DataBind();
    }

   
   
    #endregion
}