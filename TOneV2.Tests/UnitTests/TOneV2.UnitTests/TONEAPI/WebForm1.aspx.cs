﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TONEAPI.ClassCode;
//using Selenium;

//using OpenQA.Selenium;
//using OpenQA;
//using OpenQA.Selenium.Support.PageObjects;
//using OpenQA.Selenium.Chrome;
namespace TONEAPI
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Button1_Click(object sender, EventArgs e)
        {

            string testresults = "";
            /* Authentication */
            TextBox4.Text = TextBox4.Text + "Authentication \n";
            string url = TextBox1.Text;
            string data = "{\"Email\":\"" + TextBox2.Text.Trim() + "\",  \"Password\":\"" + TextBox3.Text.Trim() + "\"}";
            string data2 = "{\"Email\":\"" + TextBox2.Text + "ss\",  \"Password\":\"" + TextBox3.Text + "\"}";
            string data3 = "{\"Email\":\"" + TextBox2.Text + "\",  \"Password\":\"" + TextBox3.Text + "ss\"}";
            string endPoint = url + @"/api/VR_Sec/Security/Authenticate";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: data);


            authenticateclass lg = new authenticateclass();
            string res = lg.Testauthentication(client, url, data, data2, data3);
            TextBox7.Text = lg.getauthenticationtoken(client, url, data);

            testresults = testresults + res;
            string x = "";
            /* Countries */
            testresults = testresults + "\n --------------------------------------------- |\n|";
            TextBox4.Text = TextBox4.Text + "Countries \n";

            countries coun = new countries();

            x = coun.getcountires(client, new Uri(url + "/api/VRCommon/Country/GetCountriesInfo"), TextBox7.Text);

            testresults = testresults + x;
            try
            {
                x = coun.createcountry(client, new Uri(url + "/api/VRCommon/Country/AddCountry"), TextBox7.Text, "{\"CountryId\":\"0\",  \"Name\":\"Batatasf\"}");
                testresults = testresults + "Success: Create new Country " + x + "\n|";
            }
            catch
            {

                testresults = testresults + "Failed: Create Country|";
            }



            /*Cities*/
            TextBox4.Text = TextBox4.Text + "Cities \n";
            testresults = testresults + "\n ---------------------------------------------\n";
            cities _cities = new cities();
            string _Citiesresult = _cities.getcities(TextBox7.Text);
            testresults = testresults + _Citiesresult;

            string _newCity = "{\"CityId\":\"0\",\"Name\":\"Liber22\",\"CountryId\":\"6\"}";
            string raddcity = _cities.Addcity(_newCity, TextBox7.Text);
            testresults = testresults + "Success: Create new City " + raddcity;
            testresults = testresults + "\n ---------------------------------------------\n|";

            /* SellingNumberPlan*/
            TextBox4.Text = TextBox4.Text + "Selling Number Plan \n";

            SellingNumberPlanCode sc = new SellingNumberPlanCode();
            string _NPpostdata = "{\"Query\":{},\"SortByColumnName\":\"Entity.SellingNumberPlanId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":30}";
            string _NPresult = sc.GetSellingNunmberPlan(TextBox7.Text, "/api/WhS_BE/SellingNumberPlan/GetFilteredSellingNumberPlans", _NPpostdata);
            testresults = testresults + "Success: Selling Number Plan " + _NPresult + "\n|";

            string _newSellingNumberPlan = "{\"SellingNumberPlanId\":0,\"Name\":\"test3\"}";
            string raddSellingNP = sc.AddSellingNumberPlan(_newSellingNumberPlan, TextBox7.Text, "/api/WhS_BE/SellingNumberPlan/AddSellingNumberPlan");
            testresults = testresults + "Success: create Selling Number Plan " + raddSellingNP + "\n|";



            testresults = testresults + "\n ---------------------------------------------\n";

            testresults = testresults + _NPresult;

            /* Carrier Profiles */
            testresults = testresults + "\n ---------------------------------------------\n";
            TextBox4.Text = TextBox4.Text + "Carrier Profiles \n|";
            CarrierProfiles cp = new CarrierProfiles();
            try
            {
                x = cp.getprofiles(client, new Uri(url + "/api/WhS_BE/CarrierProfile/GetFilteredCarrierProfiles "), TextBox7.Text);

                testresults = testresults + x;
            }
            catch
            {

                testresults = testresults + "Failed: Get Carrier Profile Data|";
            }

            // create profile

            string parms = "{\"CarrierProfileId\":0,\"Name\":\"Test\",\"Settings\":{\"CountryId\":1,\"CityId\":9,\"Company\":\"Test\",\"Website\":\"123\",\"RegistrationNumber\":\"123\",\"Address\":\"123\",\"PostalCode\":\"123\",\"Town\":\"123\",\"CompanyLogo\":0,\"Contacts\":[{\"Type\":1,\"Description\":\"nab\"},{\"Type\":2,\"Description\":\"nab@vanrise.com\"},{\"Type\":4,\"Description\":\"nab\"},{\"Type\":5,\"Description\":\"nab@vanrise.com\"},{\"Type\":6,\"Description\":\"nab\"},{\"Type\":7,\"Description\":\"nab@vanrise.com\"},{\"Type\":8,\"Description\":\"nab\"},{\"Type\":9,\"Description\":\"nab@vanrise.com\"},{\"Type\":10,\"Description\":\"nab\"},{\"Type\":11,\"Description\":\"nab@vanrise.com\"},{\"Type\":12,\"Description\":\"nab\"},{\"Type\":13,\"Description\":\"nab@vanrise.com\"},{\"Type\":14,\"Description\":\"234233\"},{\"Type\":3,\"Description\":\"nab@vanrise.com\"}]}}";
            CarrierProfiles cpt = new CarrierProfiles();
            try
            {
                x = cpt.createprofile(client, new Uri(url + "/api/WhS_BE/CarrierProfile/AddCarrierProfile"), TextBox7.Text, parms);
                testresults = testresults + "Success: create Carrier Profile";
                testresults = testresults + x;
                testresults = testresults + "---------------------------------------------\n|";
            }

            catch
            {

                testresults = testresults + "Fail: create Carrier Profile |";
            }
            /* Carrier Account */

            testresults = testresults + "---------------------------------------------\n|";
            TextBox4.Text = TextBox4.Text + "Carrier Account \n";
            Carrieraccounts ca = new Carrieraccounts();
            try
            {
                x = ca.getaccounts(client, new Uri(url + "/api/WhS_BE/CarrierAccount/GetFilteredCarrierAccounts"), TextBox7.Text);
                testresults = testresults + "\n ---------------------------------------------\n|";
                testresults = testresults + x;
            }
            catch
            {

                testresults = testresults + "Failed: Get Carrier accounts Data|";
            }


            string acountparm = "{\"CarrierAccountId\":0,\"NameSuffix\":\"test\",\"AccountType\":1,\"CarrierProfileId\":1,\"SellingNumberPlanId\":1,\"SupplierSettings\":{},\"CustomerSettings\":{},\"CarrierAccountSettings\":{\"ActivationStatus\":0,\"CurrencyId\":1,\"Mask\":\"test\"}}";
            try
            {
                x = ca.createaccount(client, new Uri(url + "/api/WhS_BE/CarrierAccount/AddCarrierAccount"), TextBox7.Text, acountparm);
                testresults = testresults + "Success: Create Carrier Account \n";
                testresults = testresults + x;
            }
            catch
            {
                testresults = testresults + "Failed: Create  Carrier Account \n";
                testresults = testresults + "Failed :" + x;
            }

            // Switch 

            testresults = testresults + "---------------------------------------------\n|";
            TextBox4.Text = TextBox4.Text + "Switches \n";
            Switches sw = new Switches();
            try
            {
                x = sw.getswitches(client, new Uri(url + "/api/WhS_BE/Switch/GetFilteredSwitches"), TextBox7.Text);
                testresults = testresults + "\n ---------------------------------------------\n|";
                testresults = testresults + x;
            }
            catch
            {

                testresults = testresults + "Failed: Get Switch Data|";
            }


            try
            {  string swdataparam ="{\"SwitchId\":0,\"Name\":\"Nokia\"}";
            x = sw.createswitch(client, new Uri(url + "/api/WhS_BE/Switch/AddSwitch"), TextBox7.Text, swdataparam);
               testresults = testresults + "\n Success Create new switch";
               testresults = testresults + x;
            }

            catch
            {
                testresults = testresults + "Failed: create Switch Data|";

            }
            /*CodeGroup*/

            TextBox4.Text = TextBox4.Text + "CodeGroup \n";
            testresults = testresults + "\n ---------------------------------------------\n";
            codegroup cg = new codegroup();
            string Codegroupresult = cg.getcodegroup(TextBox7.Text);
            testresults = testresults + Codegroupresult;
            TextBox5.Text = TextBox5.Text + testresults;



            string newcg = "{\"Code\":\"9637\",\"CodeGroupId\":\"299\",\"CountryId\":\"35\"}";
             string raddcg ="";
            try
            {
                raddcg = cg.Addcodegroup(newcg, TextBox7.Text);
                testresults = testresults + "Success: Create new CodeGroup " + raddcg + "\n|";
                TextBox5.Text = TextBox5.Text + testresults;
            }
            catch
            {
                testresults = testresults + "Failed: Create new CodeGroup " + raddcg + "\n|";

            }



            // Purchase Entity

            // Supplier Zones ...
            TextBox4.Text = TextBox4.Text + "Supplier Zones\n";
            testresults = testresults + "\n ---------------------------------------------\n";
            SupplierZones sz = new SupplierZones();
            try
            {
                testresults = testresults + sz.getfiltercarriers(client, new Uri(url + "/api/WhS_BE/CarrierAccount/GetCarrierAccountInfo?serializedFilter=%7B%22GetCustomers%22:false,%22GetSuppliers%22:true%7D"), TextBox7.Text, "");
            }
            catch
            {
                testresults = testresults + "Failed: get Filtered carrier for supplier zones " + raddcg + "\n|";

            }

            try
            {
                testresults = testresults + sz.getsupplierzones(client, new Uri(url + "/api/WhS_BE/SupplierZone/GetFilteredSupplierZones"), TextBox7.Text, "");
            }
            catch
            {
                testresults = testresults + "Failed: get zoness for supplier zones " + raddcg + "\n|";

            }



            // Supplier Codes ...
            TextBox4.Text = TextBox4.Text + "Supplier Codes\n";
            testresults = testresults + "\n ---------------------------------------------\n";
            SupplierCodes scode = new SupplierCodes();
            try
            {
                testresults = testresults + scode.getfiltercarriers(client, new Uri(url + "/api/WhS_BE/CarrierAccount/GetCarrierAccountInfo?serializedFilter=%7B%22GetCustomers%22:false,%22GetSuppliers%22:true%7D"), TextBox7.Text, "");
            }
            catch
            {
                testresults = testresults + "Failed: get Filtered carrier for supplier Codes " + raddcg + "\n|";

            }

            try
            {
                testresults = testresults + scode.getsupplierCodes(client, new Uri(url + "/api/WhS_BE/SupplierCode/GetFilteredSupplierCodes"), TextBox7.Text, "");
            }
            catch
            {
                testresults = testresults + "Failed: get Codes for supplier Codes " + raddcg + "\n|";

            }


            // supplier RAte


            SupplierRates sr = new SupplierRates();
            try
            {
                client.EndPoint = url + "/api/WhS_BE/SupplierRate/GetFilteredSupplierRates";
                client.PostData = "{\"Query\":{\"SupplierId\":55,\"EffectiveOn\":\"2016-05-31T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SupplierRateId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}";
                sr.getsupplierrate(client, new Uri("http://192.168.110.195:8586/api/WhS_BE/SupplierRate/GetFilteredSupplierRates"), TextBox7.Text, "{\"Query\":{\"SupplierId\":55,\"EffectiveOn\":\"2016-05-31T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SupplierRateId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}");
            }
            catch
            {

                testresults = testresults + " Failed:";
            }


            // Sale Zones


       
            try
            {
                string _SZapi = "/api/WhS_BE/SaleZone/GetFilteredSaleZones";
                string _SZpostdata = "{\"Query\":{\"SellingNumberId\":1,\"EffectiveOn\":\"2016-05-31T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SaleZoneId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":30}";
                SaleZoneProcess _sz = new SaleZoneProcess();
                string _szResult = _sz.GetSaleZones(TextBox7.Text, _SZapi, _SZpostdata);
                testresults= testresults + _szResult;
            }
            catch
            {

                testresults = testresults + " Failed:";
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
            foreach (string s in results)
            {
                if (s.ToLower().Contains("success"))
                {
                    TextBox5.Text = TextBox5.Text + s + "\n";

                }
                else if (s.ToLower().Contains("fail"))
                {
                    TextBox6.Text = TextBox6.Text + s + "\n";

                }
            }
        }
        protected void Button2_Click(object sender, EventArgs e)
        {// TODO Auto-generated method stub
            //// load the driver for the web browser
            //OpenQA.Selenium.IWebDriver driver = new OpenQA.Selenium.Firefox.FirefoxDriver();
            ////	WebDriver driver = new FirefoxDriver();
            ////System.setProperty("webdriver.chrome.driver", "D:\\Software installed\\chromedriver.exe");
            //driver.Navigate().GoToUrl("http://192.168.110.195:8585");

            //// tell the web driver to load the page
            //// driver.Get("http://192.168.110.195:8585");

            //var element = driver.FindElement(By.XPath("//input[@placeholder=\"Username\"]"));
            //element.SendKeys("test");
            //driver.FindElement(By.XPath("//input[@placeholder=\"Username\"]")).SendKeys("admin@vanrise.com");


            ////  HtmlElementCollection htmlcol = webBrowser1.Document.GetElementsByTagName("input");

            ////foreach (HtmlElement he in htmlcol)
            ////{
            ////    if (he.innerHtml.Contains("Username"))

            ////        he.SetAttribute("value", yourstring);
            ////    break;
            //    //driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys("1");
            //    //driver.FindElement(By.XPath("//*[@class='btn btn-danger login-btn']")).Click();
           

            }
    }
}