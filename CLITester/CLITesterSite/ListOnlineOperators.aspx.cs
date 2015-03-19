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
using System.Net.Mail;
using System.Web.Configuration;
using System.Configuration;

public partial class ListOnlineOperators : BasePage
{
    public class OnlineOperator
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string mcc { get; set; }
        public string mnc { get; set; }
        public string Name { get; set; }
        public string CountryPicture { get; set; }
        public bool IsOnline { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        if (!IsPostBack)
        {
            //Call the Android service to get the list of operators
            ServiceReference1.ServiceAuthHeader auth = new ServiceReference1.ServiceAuthHeader();
            ServiceReference1.clireporterSoapClient cc = new ServiceReference1.clireporterSoapClient();
            ServiceReference1.CountryCarrier[] resp;
            
            auth.Username = "user";
            auth.Password = "idsP@ssw0rdids";

            resp = cc.GetMMCandMNC(auth, "user", "idsP@ssw0rdids");

            List<Operator> Operators = OperatorRepository.GetOperators().OrderByDescending(l => l.Id).ToList();
            List<OnlineOperator> LstOnOperators = new List<OnlineOperator>();

            foreach (Operator op in Operators)
            {
                OnlineOperator oop = new OnlineOperator();
                oop.Id = op.Id;
                oop.Name = op.Name;
                oop.mcc = op.mcc;
                oop.mnc = op.mnc;
                oop.Country = op.Country;
                oop.CountryPicture = op.CountryPicture;
                for (int i = 0; i < resp.Count(); i++)
                {
                    if (resp[i].mcc == op.mcc && resp[i].mnc == op.mnc)
                    {
                        if (resp[i].status == "Online")
                            oop.IsOnline = true;
                        else
                            oop.IsOnline = false;
                        break;
                    }
                }
                LstOnOperators.Add(oop);
            }


            rptSchedules.DataSource = LstOnOperators;
            rptSchedules.DataBind();
        }
    }


    public string GetURL()
    {
        return ConfigurationSettings.AppSettings["pathImg"];
    }
}