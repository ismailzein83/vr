using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS.Plugins.Framework.Pages
{
    public class SamplePage : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            Response.Write(
                string.Format(
                    @"
                    Requested Url: {0} <br/>
                    Page Type: {1} <br/>    
                    ", 
                    Request.RawUrl, 
                    Page.GetType().FullName
                    )
            );
            base.OnLoad(e);
        }
    }
}
