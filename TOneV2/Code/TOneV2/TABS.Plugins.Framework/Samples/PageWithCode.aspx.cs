﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ModulePages_PageWithCode : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnTest_Clicked(object sender, EventArgs e)
    {
        Response.Redirect("TABS.Plugins.Framework.Samples.SamplePage.aspx");
    }
}
