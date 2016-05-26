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

            string testresults="";
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

            testresults= testresults + res;

            /* Countries */
            testresults = testresults + "\n --------------------------------------------- |\n|";
            TextBox4.Text = TextBox4.Text + "Countries \n";
            string x = "";
            countries coun = new countries();
        
            x = coun.getcountires(client, new Uri( url +"/api/VRCommon/Country/GetCountriesInfo"), TextBox7.Text);

            testresults =testresults+ x;
            try
            {
                x = coun.createcountry(client, new Uri(url + "/api/VRCommon/Country/AddCountry"), TextBox7.Text, "{\"CountryId\":\"0\",  \"Name\":\"Batatasf\"}");
                testresults =testresults+ "Success: Create new Country " + x + "\n|";
            }
            catch
            {

                testresults =testresults + "Failed: Create Country|";
            }


            /*Cities*/
            TextBox4.Text = TextBox4.Text + "Cities";
            testresults =testresults+ "\n ---------------------------------------------\n";
            cities _cities = new cities();
            string _Citiesresult = _cities.getcities(TextBox7.Text);
            testresults =testresults+ _Citiesresult;

            string _newCity = "{\"CityId\":\"0\",\"Name\":\"Liber22\",\"CountryId\":\"6\"}";
            string raddcity = _cities.Addcity(_newCity, TextBox7.Text);
            testresults =testresults+ "Success: Create new City " + raddcity + "\n|"; 

            /* Carrier Profiles */
            testresults =testresults+ "\n ---------------------------------------------\n";
            TextBox4.Text = TextBox4.Text + "Carrier Profiles \n|";
            CarrierProfiles cp = new CarrierProfiles();
            try
            {
                x = cp.getprofiles(client, new Uri(url + "/api/WhS_BE/CarrierProfile/GetFilteredCarrierProfiles "), TextBox7.Text);

                testresults =testresults+ x;
            }
            catch
            {

                testresults =testresults + "Failed: Get Carrier Profile Data|";
            }

          // create profile
            string parms =  "{\"CarrierProfileId\":0,\"Name\":\"Test\",\"Settings\":{\"CountryId\":1,\"CityId\":9,\"Company\":\"Test\",\"Website\":\"123\",\"RegistrationNumber\":\"123\",\"Address\":\"123\",\"PostalCode\":\"123\",\"Town\":\"123\",\"CompanyLogo\":0,\"Contacts\":[{\"Type\":1,\"Description\":\"nab\"},{\"Type\":2,\"Description\":\"nab@vanrise.com\"},{\"Type\":4,\"Description\":\"nab\"},{\"Type\":5,\"Description\":\"nab@vanrise.com\"},{\"Type\":6,\"Description\":\"nab\"},{\"Type\":7,\"Description\":\"nab@vanrise.com\"},{\"Type\":8,\"Description\":\"nab\"},{\"Type\":9,\"Description\":\"nab@vanrise.com\"},{\"Type\":10,\"Description\":\"nab\"},{\"Type\":11,\"Description\":\"nab@vanrise.com\"},{\"Type\":12,\"Description\":\"nab\"},{\"Type\":13,\"Description\":\"nab@vanrise.com\"},{\"Type\":14,\"Description\":\"234233\"},{\"Type\":3,\"Description\":\"nab@vanrise.com\"}]}}";
            CarrierProfiles cpt = new CarrierProfiles();
            try
            {
                x = cpt.createprofile(client, new Uri(url + "/api/WhS_BE/CarrierProfile/AddCarrierProfile"), TextBox7.Text, parms);
                testresults = testresults + "Success: create Carrier Profile |";
                testresults = testresults + x;
            }

            catch
            {

                testresults = testresults + "Fail: create Carrier Profile |";
            }
            /* Carrier Account */

            testresults =testresults+ "\n ---------------------------------------------\n|";
            TextBox4.Text = TextBox4.Text + "Carrier Account \n";
            Carrieraccounts ca = new Carrieraccounts();
            try
            {
                x = ca.getaccounts(client, new Uri(url + "/api/WhS_BE/CarrierAccount/GetFilteredCarrierAccounts"), TextBox7.Text);

                testresults =testresults+ x;
            }
            catch
            {

                testresults =testresults + "Failed: Get Carrier accounts Data|";
            }




         






      // print results
            string[] toproing = parseresult(testresults);
            printresults(toproing);
        }


         public string[] parseresult(string results)
{

  return results.Split('|');
}

        public void printresults(string[] results)
         {
             TextBox5.Text = "";
             TextBox6.Text = "";
          //   TextBox4.Text = "";
            foreach( string s in results)
            {
                if(s.ToLower().Contains("success"))
                {
                    TextBox5.Text = TextBox5.Text + s + "\n";

                }
                else if (s.ToLower().Contains("fail"))
                {
                    TextBox6.Text = TextBox6.Text + s + "\n";

                }
            }
         }
    }


}