using System;

public partial class Home : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SetCaptions();
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = "Home";
    }
}