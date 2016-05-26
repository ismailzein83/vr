using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TONEAPI.ClassCode;

namespace TONEAPI
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

     
 protected void Button1_Click(object sender, EventArgs e)
        {

       
            /* Authentication */
            TextBox4.Text = TextBox4.Text + "Authentication \n";
            string url = TextBox1.Text;
            string data = "{\"Email\":\"" + TextBox2.Text.Trim() + "\",  \"Password\":\"" + TextBox3.Text.Trim() + "\"}";
            string data2 = "{\"Email\":\"" + TextBox2.Text + "ss\",  \"Password\":\"" + TextBox3.Text + "\"}";
            string data3 = "{\"Email\":\"" + TextBox2.Text + "\",  \"Password\":\"" + TextBox3.Text + "ss\"}";
            string endPoint =  url + @"/api/VR_Sec/Security/Authenticate";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: data);


            authenticateclass lg = new authenticateclass();
            string res = lg.Testauthentication(client,url,data,data2,data3);
            TextBox7.Text = lg.getauthenticationtoken(client, url, data);
            
            TextBox5.Text = res;

            /* Countries */
            TextBox4.Text = TextBox4.Text + "Countries \n";
            string x = "";
            countries coun = new countries();
        
            x = coun.getcountires(client, new Uri( url +"/api/VRCommon/Country/GetCountriesInfo"), TextBox7.Text);

            TextBox5.Text = TextBox5.Text + x;
            try
            {
                x = coun.createcountry(client, new Uri(url + "/api/VRCommon/Country/AddCountry"), TextBox1.Text, "{\"CountryId\":\"0\",  \"Name\":\"Batatasf\"}");
                TextBox5.Text = TextBox5.Text + "Create new Country " + x + "\n";
            }
            catch
            {

                TextBox6.Text = TextBox6.Text + "Failed: Create Country";
            }


            /*Cities*/

            cities _cities = new cities();
            string _Citiesresult = _cities.getcities(TextBox7.Text);
            TextBox5.Text = TextBox5.Text + _Citiesresult;

            string _newCity = "{\"CityId\":\"0\",\"Name\":\"Liber2\",\"CountryId\":\"6\"}";
            string raddcity = _cities.Addcountry(_newCity, TextBox7.Text);
            TextBox5.Text = TextBox5.Text + "Create new City " + raddcity + "\n"; 

     /* Carrier Profiles */
            TextBox4.Text = TextBox4.Text + "Carrier Profiles \n";
            CarrierProfiles cp = new CarrierProfiles();
            try
            {
                x = cp.getprofiles(client, new Uri(url + "/api/WhS_BE/CarrierProfile/GetFilteredCarrierProfiles "), TextBox7.Text);

                TextBox5.Text = TextBox5.Text + x;
            }
            catch
            {

                TextBox6.Text = TextBox6.Text + "Failed: Get Carrier Profile Data";
            }

            /* Carrier Account */
            TextBox4.Text = TextBox4.Text + "Carrier Account \n";
            Carrieraccounts ca = new Carrieraccounts();
            try
            {
                x = ca.getaccounts(client, new Uri(url + "/api/WhS_BE/CarrierAccount/GetFilteredCarrierAccounts"), TextBox7.Text);

                TextBox5.Text = TextBox5.Text + x;
            }
            catch
            {

                TextBox6.Text = TextBox6.Text + "Failed: Get Carrier accounts Data";
            }
        }
    }
}