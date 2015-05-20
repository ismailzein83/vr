﻿using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TOne.Web.Reports
{
    public partial class ReportChart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ReportChart.rdlc");





                List<ReportModelMatirx> datamatrix = new List<ReportModelMatirx>();

                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "January", Attemps = 200 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "January", Attemps = 152 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "January", Attemps = 322 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "January", Attemps = 11 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "January", Attemps = 112 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "January", Attemps = 595 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "February", Attemps = 545 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "February", Attemps = 632 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "February", Attemps = 154 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "February", Attemps = 132 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "February", Attemps = 119 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "February", Attemps = 15787 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "March", Attemps = 502 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "March", Attemps = 247 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "March", Attemps = 986 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "March", Attemps = 696 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "March", Attemps = 757 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "March", Attemps = 125 });

                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "April", Attemps = 200 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "April", Attemps = 152 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "April", Attemps = 322 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "April", Attemps = 11 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "April", Attemps = 112 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "April", Attemps = 595 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "May", Attemps = 545 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "May", Attemps = 632 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "May", Attemps = 154 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "May", Attemps = 132 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "May", Attemps = 119 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "May", Attemps = 787 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "June", Attemps = 502 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "June", Attemps = 247 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "June", Attemps = 986 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "June", Attemps = 696 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "June", Attemps = 757 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "June", Attemps = 125 });

                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "July", Attemps = 200 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "July", Attemps = 152 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "July", Attemps = 322 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "July", Attemps = 11 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "July", Attemps = 112 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "July", Attemps = 595 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "August", Attemps = 545 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "August", Attemps = 632 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "August", Attemps = 154 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "August", Attemps = 132 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "August", Attemps = 119 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "August", Attemps = 787 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "September", Attemps = 502 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "September", Attemps = 247 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "September", Attemps = 986 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "September", Attemps = 696 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "September", Attemps = 757 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "September", Attemps = 125 });

                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "October", Attemps = 200 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "October", Attemps = 152 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "October", Attemps = 322 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "October", Attemps = 11 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "October", Attemps = 112 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "October", Attemps = 595 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "November", Attemps = 545 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "November", Attemps = 632 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "November", Attemps = 154 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "November", Attemps = 132 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "November", Attemps = 119 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "November", Attemps = 787 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Mtc", Month = "December", Attemps = 502 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Lebanon-Alfa", Month = "December", Attemps = 247 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile", Month = "December", Attemps = 986 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Afghanistan Mobile8", Month = "December", Attemps = 696 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-MTN", Month = "December", Attemps = 757 });
                datamatrix.Add(new ReportModelMatirx { ZoneName = "Syria-Mobile-Syriatel", Month = "December", Attemps = 125 });






                ReportDataSource matrixdata = new ReportDataSource("MatrixModel", datamatrix);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(matrixdata);
            }
        }
    }
}