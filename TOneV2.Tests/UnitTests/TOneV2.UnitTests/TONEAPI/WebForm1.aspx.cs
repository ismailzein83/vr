using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TONEAPI.ClassCode;
//using Selenium;
using OpenQA.Selenium;
using OpenQA;
using OpenQA.Selenium.Support.PageObjects;
using System.Data;
using System.Drawing;
//using OpenQA.Selenium.Chrome;
namespace TONEAPI
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public int waittime;
       

        protected string gotomenu(string menuitem)
          {
              string ret = "";
        connect con = new connect();
        string cons = "Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=no@cce$$dev";
        DataSet ds = con.getdatatesting("select [values] as vv from menuurl where item='" + menuitem + "'",cons);
         foreach ( DataRow _r in ds.Tables[0].Rows)
         {
             ret= _r["vv"].ToString();
         }
         return ret;
          }
        protected void Page_Load(object sender, EventArgs e)
        {
            DropDownList1.Items.Insert(0, new ListItem("Login", "Login"));
            DropDownList1.Items.Insert(0, new ListItem("Countries", "Countries"));
            DropDownList1.Items.Insert(0, new ListItem("Cities", "Cities"));
            DropDownList1.Items.Insert(0, new ListItem("Carrier profile", "Carrierprofiles"));
            DropDownList1.Items.Insert(0, new ListItem("Carrier account", "Carrieraccounts"));
            DropDownList1.Items.Insert(0, new ListItem("Code Groups", "Code Groups"));
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
           // rest();
            Response.Redirect("webform2.aspx");
            bindgrid();
        }

        public void bindgrid()
        {
            string cons = "Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=no@cce$$dev";
            connect con = new connect();
            DataSet ds = con.getdatas(" select * from [ToneV2testing].[dbo].[logging]   where eventdate > dateadd(hh,-1,getdate()) order   by eventdate desc",cons);
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();

            foreach(GridViewRow GR in GridView1.Rows)
            {
                if (GR.Cells[2].Text.ToLower() == "fail")
                    GR.BackColor = Color.DarkOrange;
            }
     

        }
        public void rest()
        {
            //connect con = new connect();
            //string testresults = "";
            ///* Authentication */
            //TextBox4.Text = TextBox4.Text + "Authentication \n";
            //string url = TextBox1.Text;
            //string data = "{\"Email\":\"" + TextBox2.Text.Trim() + "\",  \"Password\":\"" + TextBox3.Text.Trim() + "\"}";
            //string data2 = "{\"Email\":\"" + TextBox2.Text + "ss\",  \"Password\":\"" + TextBox3.Text + "\"}";
            //string data3 = "{\"Email\":\"" + TextBox2.Text + "\",  \"Password\":\"" + TextBox3.Text + "ss\"}";
            //string endPoint = url + @"/api/VR_Sec/Security/Authenticate";


            //var client = new RestClient(endpoint: endPoint,
            //                method: HttpVerb.POST,
            //                contenttype: "application/json;charset=UTF-8",
            //                postData: data);


            //authenticateclass lg = new authenticateclass();
            //string res = lg.Testauthentication(client, url, data, data2, data3);
            //TextBox7.Text = lg.getauthenticationtoken(client, url, data);

            //testresults = testresults + res;
            //string x = "";
            ///* Countries */
            //testresults = testresults + "\n --------------------------------------------- |\n|";
            //TextBox4.Text = TextBox4.Text + "Countries \n";

            //countries coun = new countries();

            //x = coun.getcountires(client, new Uri(url + "/api/VRCommon/Country/GetCountriesInfo"), TextBox7.Text);

            //testresults = testresults + x;
            //try
            //{
            //    x = coun.createcountry(client, new Uri(url + "/api/VRCommon/Country/AddCountry"), TextBox7.Text, "{\"CountryId\":\"0\",  \"Name\":\"Batatasf\"}");
            //    testresults = testresults + "Success: Create new Country " + x + "\n|";
            //    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','create Country','success','" + x + "',getdate(),'API'");
            //}
            //catch
            //{
            //    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','create Country','Fail','" + x + "',getdate(),'API'");
            //    testresults = testresults + "Failed: Create Country|";
            //}



            ///*Cities*/
            //TextBox4.Text = TextBox4.Text + "Cities \n";
            //testresults = testresults + "\n ---------------------------------------------\n";
            //cities _cities = new cities();
            //string _Citiesresult = _cities.getcities(TextBox7.Text);
            //testresults = testresults + _Citiesresult;

            //string _newCity = "{\"CityId\":\"0\",\"Name\":\"Liber22\",\"CountryId\":\"6\"}";
            //string raddcity = _cities.Addcity(_newCity, TextBox7.Text);
            //testresults = testresults + "Success: Create new City " + raddcity;
            //testresults = testresults + "\n ---------------------------------------------\n|";

            ///* SellingNumberPlan*/
            //TextBox4.Text = TextBox4.Text + "Selling Number Plan \n";

            //SellingNumberPlanCode sc = new SellingNumberPlanCode();
            //string _NPpostdata = "{\"Query\":{},\"SortByColumnName\":\"Entity.SellingNumberPlanId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":30}";
            //string _NPresult = sc.GetSellingNunmberPlan(TextBox7.Text, "/api/WhS_BE/SellingNumberPlan/GetFilteredSellingNumberPlans", _NPpostdata);
            //testresults = testresults + "Success: Selling Number Plan " + _NPresult + "\n|";

            //string _newSellingNumberPlan = "{\"SellingNumberPlanId\":0,\"Name\":\"test3\"}";
            //string raddSellingNP = sc.AddSellingNumberPlan(_newSellingNumberPlan, TextBox7.Text, "/api/WhS_BE/SellingNumberPlan/AddSellingNumberPlan");
            //testresults = testresults + "Success: create Selling Number Plan " + raddSellingNP + "\n|";



            //testresults = testresults + "\n ---------------------------------------------\n";

            //testresults = testresults + _NPresult;

            ///* Carrier Profiles */
            //testresults = testresults + "\n ---------------------------------------------\n";
            //TextBox4.Text = TextBox4.Text + "Carrier Profiles \n|";
            //CarrierProfiles cp = new CarrierProfiles();
            //try
            //{
            //    x = cp.getprofiles(client, new Uri(url + "/api/WhS_BE/CarrierProfile/GetFilteredCarrierProfiles "), TextBox7.Text);

            //    testresults = testresults + x;
            //}
            //catch
            //{

            //    testresults = testresults + "Failed: Get Carrier Profile Data|";
            //}

            //// create profile

            //string parms = "{\"CarrierProfileId\":0,\"Name\":\"Test\",\"Settings\":{\"CountryId\":1,\"CityId\":9,\"Company\":\"Test\",\"Website\":\"123\",\"RegistrationNumber\":\"123\",\"Address\":\"123\",\"PostalCode\":\"123\",\"Town\":\"123\",\"CompanyLogo\":0,\"Contacts\":[{\"Type\":1,\"Description\":\"nab\"},{\"Type\":2,\"Description\":\"nab@vanrise.com\"},{\"Type\":4,\"Description\":\"nab\"},{\"Type\":5,\"Description\":\"nab@vanrise.com\"},{\"Type\":6,\"Description\":\"nab\"},{\"Type\":7,\"Description\":\"nab@vanrise.com\"},{\"Type\":8,\"Description\":\"nab\"},{\"Type\":9,\"Description\":\"nab@vanrise.com\"},{\"Type\":10,\"Description\":\"nab\"},{\"Type\":11,\"Description\":\"nab@vanrise.com\"},{\"Type\":12,\"Description\":\"nab\"},{\"Type\":13,\"Description\":\"nab@vanrise.com\"},{\"Type\":14,\"Description\":\"234233\"},{\"Type\":3,\"Description\":\"nab@vanrise.com\"}]}}";
            //CarrierProfiles cpt = new CarrierProfiles();
            //try
            //{
            //    x = cpt.createprofile(client, new Uri(url + "/api/WhS_BE/CarrierProfile/AddCarrierProfile"), TextBox7.Text, parms);
            //    testresults = testresults + "Success: create Carrier Profile";
            //    testresults = testresults + x;
            //    testresults = testresults + "---------------------------------------------\n|";
            //}

            //catch
            //{

            //    testresults = testresults + "Fail: create Carrier Profile |";
            //}
            ///* Carrier Account */

            //testresults = testresults + "---------------------------------------------\n|";
            //TextBox4.Text = TextBox4.Text + "Carrier Account \n";
            //Carrieraccounts ca = new Carrieraccounts();
            //try
            //{
            //    x = ca.getaccounts(client, new Uri(url + "/api/WhS_BE/CarrierAccount/GetFilteredCarrierAccounts"), TextBox7.Text);
            //    testresults = testresults + "\n ---------------------------------------------\n|";
            //    testresults = testresults + x;
            //}
            //catch
            //{

            //    testresults = testresults + "Failed: Get Carrier accounts Data|";
            //}


            //string acountparm = "{\"CarrierAccountId\":0,\"NameSuffix\":\"test\",\"AccountType\":1,\"CarrierProfileId\":1,\"SellingNumberPlanId\":1,\"SupplierSettings\":{},\"CustomerSettings\":{},\"CarrierAccountSettings\":{\"ActivationStatus\":0,\"CurrencyId\":1,\"Mask\":\"test\"}}";
            //try
            //{
            //    x = ca.createaccount(client, new Uri(url + "/api/WhS_BE/CarrierAccount/AddCarrierAccount"), TextBox7.Text, acountparm);
            //    testresults = testresults + "Success: Create Carrier Account \n";
            //    testresults = testresults + x;
            //}
            //catch
            //{
            //    testresults = testresults + "Failed: Create  Carrier Account \n";
            //    testresults = testresults + "Failed :" + x;
            //}

            //// Switch 

            //testresults = testresults + "---------------------------------------------\n|";
            //TextBox4.Text = TextBox4.Text + "Switches \n";
            //Switches sw = new Switches();
            //try
            //{
            //    x = sw.getswitches(client, new Uri(url + "/api/WhS_BE/Switch/GetFilteredSwitches"), TextBox7.Text);
            //    testresults = testresults + "\n ---------------------------------------------\n|";
            //    testresults = testresults + x;
            //}
            //catch
            //{

            //    testresults = testresults + "Failed: Get Switch Data|";
            //}


            //try
            //{
            //    string swdataparam = "{\"SwitchId\":0,\"Name\":\"Nokia\"}";
            //    x = sw.createswitch(client, new Uri(url + "/api/WhS_BE/Switch/AddSwitch"), TextBox7.Text, swdataparam);
            //    testresults = testresults + "\n Success Create new switch";
            //    testresults = testresults + x;
            //}

            //catch
            //{
            //    testresults = testresults + "Failed: create Switch Data|";

            //}
            ///*CodeGroup*/

            //TextBox4.Text = TextBox4.Text + "CodeGroup \n";
            //testresults = testresults + "\n ---------------------------------------------\n";
            //codegroup cg = new codegroup();
            //string Codegroupresult = cg.getcodegroup(TextBox7.Text);
            //testresults = testresults + Codegroupresult;
            //TextBox5.Text = TextBox5.Text + testresults;



            //string newcg = "{\"Code\":\"9637\",\"CodeGroupId\":\"299\",\"CountryId\":\"35\"}";
            //string raddcg = "";
            //try
            //{
            //    raddcg = cg.Addcodegroup(newcg, TextBox7.Text);
            //    testresults = testresults + "Success: Create new CodeGroup " + raddcg + "\n|";
            //    TextBox5.Text = TextBox5.Text + testresults;
            //}
            //catch
            //{
            //    testresults = testresults + "Failed: Create new CodeGroup " + raddcg + "\n|";

            //}



            //// Purchase Entity

            //// Supplier Zones ...
            //TextBox4.Text = TextBox4.Text + "Supplier Zones\n";
            //testresults = testresults + "\n ---------------------------------------------\n";
            //SupplierZones sz = new SupplierZones();
            //try
            //{
            //    testresults = testresults + sz.getfiltercarriers(client, new Uri(url + "/api/WhS_BE/CarrierAccount/GetCarrierAccountInfo?serializedFilter=%7B%22GetCustomers%22:false,%22GetSuppliers%22:true%7D"), TextBox7.Text, "");
            //}
            //catch
            //{
            //    testresults = testresults + "Failed: get Filtered carrier for supplier zones " + raddcg + "\n|";

            //}

            //try
            //{
            //    testresults = testresults + sz.getsupplierzones(client, new Uri(url + "/api/WhS_BE/SupplierZone/GetFilteredSupplierZones"), TextBox7.Text, "");
            //}
            //catch
            //{
            //    testresults = testresults + "Failed: get zoness for supplier zones " + raddcg + "\n|";

            //}



            //// Supplier Codes ...
            //TextBox4.Text = TextBox4.Text + "Supplier Codes\n";
            //testresults = testresults + "\n ---------------------------------------------\n";
            //SupplierCodes scode = new SupplierCodes();
            //try
            //{
            //    testresults = testresults + scode.getfiltercarriers(client, new Uri(url + "/api/WhS_BE/CarrierAccount/GetCarrierAccountInfo?serializedFilter=%7B%22GetCustomers%22:false,%22GetSuppliers%22:true%7D"), TextBox7.Text, "");
            //}
            //catch
            //{
            //    testresults = testresults + "Failed: get Filtered carrier for supplier Codes " + raddcg + "\n|";

            //}

            //try
            //{
            //    testresults = testresults + scode.getsupplierCodes(client, new Uri(url + "/api/WhS_BE/SupplierCode/GetFilteredSupplierCodes"), TextBox7.Text, "");
            //}
            //catch
            //{
            //    testresults = testresults + "Failed: get Codes for supplier Codes " + raddcg + "\n|";

            //}


            //// supplier RAte


            //SupplierRates sr = new SupplierRates();
            //try
            //{
            //    client.EndPoint = url + "/api/WhS_BE/SupplierRate/GetFilteredSupplierRates";
            //    client.PostData = "{\"Query\":{\"SupplierId\":55,\"EffectiveOn\":\"2016-05-31T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SupplierRateId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}";
            //    sr.getsupplierrate(client, new Uri("http://192.168.110.195:8586/api/WhS_BE/SupplierRate/GetFilteredSupplierRates"), TextBox7.Text, "{\"Query\":{\"SupplierId\":55,\"EffectiveOn\":\"2016-05-31T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SupplierRateId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}");
            //}
            //catch
            //{

            //    testresults = testresults + " Failed:";
            //}


            //// Sale Zones



            //try
            //{
            //    string _SZapi = "/api/WhS_BE/SaleZone/GetFilteredSaleZones";
            //    string _SZpostdata = "{\"Query\":{\"SellingNumberId\":1,\"EffectiveOn\":\"2016-05-31T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SaleZoneId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":30}";
            //    SaleZoneProcess _sz = new SaleZoneProcess();
            //    string _szResult = _sz.GetSaleZones(TextBox7.Text, _SZapi, _SZpostdata);
            //    testresults = testresults + _szResult;
            //}
            //catch
            //{

            //    testresults = testresults + " Failed:";
            //}




            // print results
            //string[] toproing = parseresult(testresults);
            //printresults(toproing);
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
        {
            waittime = 2000;

            login();   //Done
            codegroup();  //done
            GUIcountry();  //done
            GUICities();     //done
            carrierprofile(); //done
            //  carrieraccount();
         //   currencies();
        //   ratetypes();
         //  carrierprofile();
         //  carrieraccount();
            bindgrid();
        }

        /*   ---------------- GUI Testing ---------------*/

        public void GUIcountry()
        {
            string pagemenu = "Countries";
            string parentmenuitem = "";
            string childmenuitem = "";
            string submenuitem = "";
            connect con = new connect();


            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(TextBox1.Text);
            System.Threading.Thread.Sleep(waittime);
            // Login
            try
            {
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys(TextBox2.Text);
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys(TextBox3.Text);

                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login in success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','login in failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                driver.Close();
            }
            // go to look ups 
            try
            {
                DataSet ds1 = con.getdatas("select * from menu where page='" + pagemenu + "'", "Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=no@cce$$dev");
                foreach (DataRow _r in ds1.Tables[0].Rows)
                {
                    parentmenuitem = _r["parent"].ToString();
                    childmenuitem = _r["child"].ToString();
                    submenuitem = _r["sub"].ToString();
                }
                System.Threading.Thread.Sleep(waittime);
                System.Threading.Thread.Sleep(waittime);
                List<IWebElement> mainmenu = driver.FindElements(By.XPath("//span[@class='vr-module-name ng-binding']")).ToList();
                List<IWebElement> submenu = new List<IWebElement>();
                List<IWebElement> childmenu = new List<IWebElement>();
                List<IWebElement> submenus = new List<IWebElement>();
                List<IWebElement> submenusnotnull = new List<IWebElement>();
                List<IWebElement> childmenuss = new List<IWebElement>();
                List<IWebElement> childmenunotnull = new List<IWebElement>();
                List<IWebElement> childmenunotnull2 = new List<IWebElement>();


                try
                {
                    foreach (IWebElement element in mainmenu)
                    {
                        if (element.Text == parentmenuitem)
                        {
                            element.Click();

                            if (childmenuitem != "")
                            {
                                System.Threading.Thread.Sleep(waittime);
                                submenus = driver.FindElements(By.XPath("//div[@class='menulink waves-effect waves-block ng-binding']")).ToList();
                                submenusnotnull = submenus.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                                foreach (IWebElement childelement in submenusnotnull)
                                {
                                    if (childelement.Text == childmenuitem)
                                    {
                                        childelement.Click();
                                        System.Threading.Thread.Sleep(waittime);
                                        childmenuss = driver.FindElements(By.XPath("//a[@class='menulink ng-binding']")).ToList();
                                        childmenunotnull = childmenuss.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                                        foreach (IWebElement subelement in childmenunotnull)
                                        {
                                            if (subelement.Text == submenuitem)
                                            {
                                                subelement.Click();
                                                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','navigate to Countries','Success','navigation success',getdate(),'GUI'");
                                            }
                                        }
                                    }

                                }
                            }

                        }
                        else
                        {
                            //  con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','navigate to CodeGroup','Fail','navigation failure exception:failed to go to menu',getdate(),'GUI'");
                        }

                    }
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','navigate to Countries','Fail','navigation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                }

                // search if zone exists
                try
                {
                    var element = driver.FindElement(By.XPath(gotomenu("CountriesGrid")));
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','page browsing','Success','browsing success',getdate(),'GUI'");
                    var element2 = driver.FindElement(By.XPath(gotomenu("CountriesGrid")));
                    string x = element.Text.ToString();
                    var count = driver.FindElement(By.XPath(gotomenu("CountriesGridcount")));
                    if (count.Text.Equals("Total count (255)"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','countries count','Success','navigation success',getdate(),'GUI'");


                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','countries count in correct','Success','navigation success',getdate(),'GUI'");


                    }
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','countries count in correct','Success','navigation success exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }
                try
                {
                    // search for a specific zone
                    driver.FindElement(By.CssSelector("#mainInput")).SendKeys("Calgary");
                    driver.FindElement(By.XPath(gotomenu("Countriessearch"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    var elementg = driver.FindElement(By.XPath(gotomenu("Countriessearchvalue")));
                    System.Threading.Thread.Sleep(waittime);
                    if (elementg.Text.Equals("Calgary"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','countries search','Success','search success',getdate(),'GUI'");

                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','countries search','Fail','search failed no item found',getdate(),'GUI'");
                    }
                    //  string currentHandle = driver.CurrentWindowHandle;
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','countries search','Fail','search failed no item foundexception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                }
                try
                {
                    System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("Countriesadd"))).Click();
                    System.Threading.Thread.Sleep(waittime);


                    // add a new zone
                    var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                    var textinput = modaldiag.FindElement(By.CssSelector("#mainInput"));
                    textinput.SendKeys("Calgary");
                    System.Threading.Thread.Sleep(waittime);
                    var textsave = modaldiag.FindElement(By.XPath(gotomenu("Countriessave")));
                    
                    textsave.Click();
                    System.Threading.Thread.Sleep(waittime);
                    // driver.FindElement(By.XPath("[@class,'modal-dialog'][@id='mainInput']")).SendKeys("Calgary");
                   // driver.FindElement(By.XPath(gotomenu("Countriessave"))).Click();
                    try
                    {
                        if (!driver.FindElement(By.XPath(gotomenu("Countryexists"))).Displayed)
                        {
                            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','Countries add','Success','adding Countries success already exists',getdate(),'GUI'");
                        }
                        else
                        {
                            driver.FindElement(By.XPath(gotomenu("codegroupcloseadd"))).Click();
                        }
                      //  driver.FindElement(By.XPath(gotomenu("codegroupcloseadd"))).Click();
                    }
                    catch (Exception Ex)
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','Countries add','Success','adding Countries success',getdate(),'GUI'");
                        driver.FindElement(By.XPath(gotomenu("codegroupcloseadd"))).Click();
                    }
                 
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','countries add','Success','adding country success',getdate(),'GUI'");
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','countries add','Fail','adding country failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                    //   driver.Close();
                }

            }
            catch(Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','countries add','Fail','adding country failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
            }
            driver.Close();
        }

        public void GUICities()
        {
            string pagemenu = "Cities";
            string parentmenuitem = "";
            string childmenuitem = "";
            string submenuitem = "";
            connect con = new connect();


            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(TextBox1.Text);
            System.Threading.Thread.Sleep(waittime);
            // Login
            try
            {
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys(TextBox2.Text);
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys(TextBox3.Text);

                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login in success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','login in failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                driver.Close();
            }
            // go to look ups 
            try
            {
                DataSet ds1 = con.getdatas("select * from menu where page='" + pagemenu + "'", "Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=no@cce$$dev");
                foreach (DataRow _r in ds1.Tables[0].Rows)
                {
                    parentmenuitem = _r["parent"].ToString();
                    childmenuitem = _r["child"].ToString();
                    submenuitem = _r["sub"].ToString();
                }
                System.Threading.Thread.Sleep(waittime);
                System.Threading.Thread.Sleep(waittime);
                List<IWebElement> mainmenu = driver.FindElements(By.XPath("//span[@class='vr-module-name ng-binding']")).ToList();
                List<IWebElement> submenu = new List<IWebElement>();
                List<IWebElement> childmenu = new List<IWebElement>();
                List<IWebElement> submenus = new List<IWebElement>();
                List<IWebElement> submenusnotnull = new List<IWebElement>();
                List<IWebElement> childmenuss = new List<IWebElement>();
                List<IWebElement> childmenunotnull = new List<IWebElement>();
                List<IWebElement> childmenunotnull2 = new List<IWebElement>();


                try
                {
                    foreach (IWebElement element in mainmenu)
                    {
                        if (element.Text == parentmenuitem)
                        {
                            element.Click();

                            if (childmenuitem != "")
                            {
                                System.Threading.Thread.Sleep(waittime);
                                submenus = driver.FindElements(By.XPath("//div[@class='menulink waves-effect waves-block ng-binding']")).ToList();
                                submenusnotnull = submenus.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                                foreach (IWebElement childelement in submenusnotnull)
                                {
                                    if (childelement.Text == childmenuitem)
                                    {
                                        childelement.Click();
                                        System.Threading.Thread.Sleep(waittime);
                                        childmenuss = driver.FindElements(By.XPath("//a[@class='menulink ng-binding']")).ToList();
                                        childmenunotnull = childmenuss.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                                        foreach (IWebElement subelement in childmenunotnull)
                                        {
                                            if (subelement.Text == submenuitem)
                                            {
                                                subelement.Click();
                                                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','navigate to Cities','Success','navigation success',getdate(),'GUI'");
                                            }
                                        }
                                    }

                                }
                            }

                        }
                        else
                        {
                            //  con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','navigate to CodeGroup','Fail','navigation failure exception:failed to go to menu',getdate(),'GUI'");
                        }

                    }
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','navigate to Cities','Fail','navigation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                }


                // search if zone exists
                try
                {
                    var element = driver.FindElement(By.XPath(gotomenu("Citiesgrid")));
                    System.Threading.Thread.Sleep(waittime);
                    var element2 = driver.FindElement(By.XPath(gotomenu("Citiesgrid")));
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','City found','Success','Find city success',getdate(),'GUI'");
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','City not found','Fail','Find city Fail exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }

                try
                {
                    var count = driver.FindElement(By.XPath(gotomenu("Citiesgridcount")));
                    if (count.Text.Equals("Total count (2)"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities count','Success','navigation success',getdate(),'GUI'");


                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities count in correct','Success','navigation success',getdate(),'GUI'");


                    }
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities count in correct','Success','navigation success exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                }
                // search for a specific zone
                try
                {
                    driver.FindElement(By.CssSelector("#mainInput")).SendKeys("Beir");
                    System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("Citiessearch"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    var elementg = driver.FindElement(By.XPath(gotomenu("Citiesgrid")));

                    if (elementg.Text.Equals("Beirut"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities search','Success','search success',getdate(),'GUI'");

                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities search','Fail','search failed no item found',getdate(),'GUI'");
                    }
                    //  string currentHandle = driver.CurrentWindowHandle;
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities search','Fail','search failed no item found exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }
                try
                {
                    driver.FindElement(By.XPath(gotomenu("CitiesAdd"))).Click();
                    System.Threading.Thread.Sleep(waittime);




                    // add a new zone
                    var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                    System.Threading.Thread.Sleep(waittime);
                    var textinput = modaldiag.FindElement(By.CssSelector("#mainInput"));
                    System.Threading.Thread.Sleep(waittime);
                    textinput.SendKeys("Albania New");
                    System.Threading.Thread.Sleep(waittime);
                    var textsave = modaldiag.FindElement(By.CssSelector("body > div.modal.ng-scope.top.am-fade-and-scale > div > div > div:nth-child(2) > div:nth-child(2) > div > vr-actionbar > div > div:nth-child(2) > div > div > div > vr-button:nth-child(1) > div > button"));
                    System.Threading.Thread.Sleep(waittime);
                    textsave.Click();
                    System.Threading.Thread.Sleep(waittime);
                    var count = modaldiag.FindElement(By.CssSelector("body > div.modal.ng-scope.top.am-fade-and-scale > div > div > div:nth-child(2) > div:nth-child(2) > vr-modalbody > div > vr-form > div > vr-validation-group > vr-row > div > span > vr-columns > div > vr-common-country-selector > vr-columns > div > vr-select > div > div > vr-validator > div > div:nth-child(1) > div > button"));
                    System.Threading.Thread.Sleep(waittime);
                    count.Click();
                    System.Threading.Thread.Sleep(waittime);
                    // driver.FindElement(By.XPath("[@class,'modal-dialog'][@id='mainInput']")).SendKeys("Calgary");
                    //driver.FindElement(By.XPath("/html/body/div[4]/div/div/div[2]/div[2]/div/vr-actionbar/div/div[2]/div/div/div/vr-button[1]/div/button")).Click();
                    var selelement = modaldiag.FindElement(By.XPath(gotomenu("Citiesselectcountry")));
                    System.Threading.Thread.Sleep(waittime);
                    selelement.Click();
                    System.Threading.Thread.Sleep(waittime);
                    var save = modaldiag.FindElement(By.XPath(gotomenu("Citiessave")));
                    System.Threading.Thread.Sleep(waittime);
                    save.Click();


                    driver.Close();
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities add','Success','adding Cities success',getdate(),'GUI'");
                }
                

                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities add','Fail','adding Cities failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities add','Fail','adding Cities failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
        }

        public void login()
        {
            connect con = new connect();
            try
            {


                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                ChromeDriver driver = new ChromeDriver(options);
                string x = "";
                driver.Navigate().GoToUrl("http://192.168.110.195:8103");
                System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys("admin@vanrise.com");
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys("11");
                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                try
                {
                    var element = driver.FindElement(By.CssSelector("body > div.cg-notify-message.ng-scope.alert.alert-danger.cg-notify-message-center > div.ng-binding"));
                    x = element.Text.ToString();
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','login in failed',getdate(),'GUI'");
                }
                catch
                {
                    driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                    var element = driver.FindElement(By.CssSelector("body > div.cg-notify-message.ng-scope.alert.alert-danger.cg-notify-message-center > div.ng-binding"));
                    x = element.Text.ToString();
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login wrong credentials',getdate(),'GUI'");
                }


                driver.Close();
             
                if (x.Equals("Login Failed. Wrong Credentials"))

                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','Authentication success',getdate(),'GUI'");
                else
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','Authentication Failure',getdate(),'GUI'");
            }
            catch
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','Fail processing login',getdate(),'GUI'");
            }
            // correct login 

            try
            {
                ChromeDriver driver = new ChromeDriver();
                driver.Navigate().GoToUrl("http://192.168.110.195:8103");
                System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys("admin@vanrise.com");
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys("1");
                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                driver.Close();
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login Success',getdate(),'GUI'");
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','Authentication success',getdate(),'GUI'");
            }
            catch
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','Fail processing login',getdate(),'GUI'");

            }
        }

        // to be written
        public void codegroup()
        {
            string pagemenu = "codegroups";
            string parentmenuitem = "";
            string childmenuitem = "";
            string submenuitem = "";
            connect con = new connect();


            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(TextBox1.Text);
            System.Threading.Thread.Sleep(waittime);
            // Login
            try
            {
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys(TextBox2.Text);
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys(TextBox3.Text);

                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login in success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','login in failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                driver.Close();
            }
            // go to look ups 
            try
            {
                DataSet ds1 = con.getdatas("select * from menu where page='" + pagemenu + "'", "Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=no@cce$$dev");
                foreach (DataRow _r in ds1.Tables[0].Rows)
                {
                    parentmenuitem = _r["parent"].ToString();
                    childmenuitem = _r["child"].ToString();
                    submenuitem = _r["sub"].ToString();
                }
                System.Threading.Thread.Sleep(waittime);
                System.Threading.Thread.Sleep(waittime);
                List<IWebElement> mainmenu = driver.FindElements(By.XPath("//span[@class='vr-module-name ng-binding']")).ToList();
                List<IWebElement> submenu = new List<IWebElement>();
                List<IWebElement> childmenu = new List<IWebElement>();
                List<IWebElement> submenus = new List<IWebElement>();
                List<IWebElement> submenusnotnull = new List<IWebElement>();
                List<IWebElement> childmenuss = new List<IWebElement>();
                List<IWebElement> childmenunotnull = new List<IWebElement>();
                List<IWebElement> childmenunotnull2 = new List<IWebElement>();


                try
                {
                    foreach (IWebElement element in mainmenu)
                    {
                        if (element.Text == parentmenuitem)
                        {
                            element.Click();
                           
                            if (childmenuitem != "")
                            {
                                System.Threading.Thread.Sleep(waittime);
                                submenus = driver.FindElements(By.XPath("//div[@class='menulink waves-effect waves-block ng-binding']")).ToList();
                                submenusnotnull = submenus.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                                foreach (IWebElement childelement in submenusnotnull)
                                {
                                    if (childelement.Text == childmenuitem)
                                    {
                                        childelement.Click();
                                        System.Threading.Thread.Sleep(waittime);
                                        childmenuss = driver.FindElements(By.XPath("//a[@class='menulink ng-binding']")).ToList();
                                        childmenunotnull = childmenuss.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                                        foreach (IWebElement subelement in childmenunotnull)
                                        {
                                            if (subelement.Text == submenuitem)
                                            {
                                                subelement.Click();
                                                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','navigate to CodeGroup','Success','navigation success',getdate(),'GUI'");
                                            }
                                        }
                                    }

                                }
                            }

                        }
                        else
                        {
                          //  con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','navigate to CodeGroup','Fail','navigation failure exception:failed to go to menu',getdate(),'GUI'");
                        }

                    }
                }
                catch(Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','navigate to CodeGroup','Fail','navigation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                }


                // search if zone exists
                try
                {
                    var element = driver.FindElement(By.XPath(gotomenu("codegroupgriditem")));
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','page browsing','Success','browsing success',getdate(),'GUI'");
                    var element2 = driver.FindElement(By.XPath(gotomenu("codegroupafghcodegroup")));
                    string x = element.Text.ToString();
                    var count = driver.FindElement(By.XPath(gotomenu("codegroupgridcount")));
                    if (count.Text.Equals("Total count (261)"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup count','Success','navigation success',getdate(),'GUI'");


                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup count in correct','Success','navigation success',getdate(),'GUI'");


                    }
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup count in correct','Success','navigation success exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }
                try
                {
                    // search for a specific zone
                    driver.FindElement(By.CssSelector("#mainInput")).SendKeys("961");
                    System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("codegroupsearchitem"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    var elementg = driver.FindElement(By.XPath(gotomenu("codegroupgetitem")));
                    System.Threading.Thread.Sleep(waittime);
                    if (elementg.Text.Equals("961"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup search','Success','search success',getdate(),'GUI'");

                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup search','Fail','search failed no item found',getdate(),'GUI'");
                    }
                    //  string currentHandle = driver.CurrentWindowHandle;
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup search','Fail','search failed no item found exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                }
                try
                {
                     driver.FindElement(By.XPath(gotomenu("codegroupaddpopup"))).Click();
                    System.Threading.Thread.Sleep(waittime);

                    // add a new zone
                    var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                    System.Threading.Thread.Sleep(waittime);
                    var textinput = modaldiag.FindElement(By.CssSelector("#mainInput"));
                    textinput.SendKeys("93222");
                    System.Threading.Thread.Sleep(waittime);
                    var textsave = modaldiag.FindElement(By.CssSelector(gotomenu("codegroupaddcountrydropdown")));
                    textsave.Click();
                    // driver.FindElement(By.XPath("[@class,'modal-dialog'][@id='mainInput']")).SendKeys("Calgary");
                    System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("codegroupaddcountryclickcontrol"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("codegroupaddsavebutton"))).Click();
                    System.Threading.Thread.Sleep(500);
                    // driver.Close();
                    try
                    {
                        if(!driver.FindElement(By.XPath(gotomenu("Codegroupalreadyexists"))).Displayed)
                        {
                            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup add','Success','adding CodeGroup success already exists',getdate(),'GUI'");
                        }
                        else
                        {
                            driver.FindElement(By.XPath(gotomenu("codegroupcloseadd"))).Click();
                        }
                        driver.FindElement(By.XPath(gotomenu("codegroupcloseadd"))).Click();
                    }
                    catch (Exception Ex)
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup add','Success','adding CodeGroup success',getdate(),'GUI'");
                        driver.FindElement(By.XPath(gotomenu("codegroupcloseadd"))).Click();
                    }
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup add','Success','adding CodeGroup success',getdate(),'GUI'");
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup add','Fail','adding CodeGroup failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                    //  driver.Close();
                }

                try
                {
                    //edit code group
                    System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("codegroupeditpressgrid"))).Click();

                    System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("codegrouoeditpopup"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                    System.Threading.Thread.Sleep(waittime);
                    var textinput = modaldiag.FindElement(By.CssSelector("#mainInput"));
                    System.Threading.Thread.Sleep(waittime);
                    var savebutton = modaldiag.FindElement(By.XPath(gotomenu("codegroupeditpopupsave")));
                    System.Threading.Thread.Sleep(waittime);
                    savebutton.Click();
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup Edit','Success','Editting CodeGroup Success',getdate(),'GUI'");
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup Edit','Fail','Editing CodeGroup failure exception:" + Ex.ToString().Replace('"', ' ').Replace('"', ' ') + "',getdate(),'GUI'");
                }
                driver.Close();


            }
            catch(Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup Edit','Fail','Editing CodeGroup failure exception:" + Ex.ToString().Replace('"', ' ').Replace('"', ' ') + "',getdate(),'GUI'");
            }
        }

        // to be written
        public void currencies()
        {
            connect con = new connect();


            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://192.168.110.195:8103");
             System.Threading.Thread.Sleep(waittime);
            // Login
            try
            {
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys("admin@vanrise.com");
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys("1");

                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                 System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login in success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','login in failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                driver.Close();
            }
            // go to look ups 
            try
            {
                driver.FindElement(By.XPath(gotomenu("BusinessEntities"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("Lookups"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("Currencies"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','navigate to Currencies','Success','navigation success',getdate(),'GUI'");
                //con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','navigate to Countries','Success','navigation success',getdate(),'GUI'");
            }

            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','navigate to Currencies','Fail','navigation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                //driver.Close();
            }
            // check data is retrieved ok and count 
            var count = driver.FindElement(By.XPath(gotomenu("Currenciesgridcount")));
            if (count.Text.Equals("Total count (174)"))
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Currencies count','Success','Data Validation',getdate(),'GUI'");


            }
            else
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Currencies count in correct','Success','Data Validation',getdate(),'GUI'");


            }
            
            // Search by name
            try
            {
                driver.FindElement(By.XPath(gotomenu("currenciessearchbyname"))).SendKeys("United States");
                driver.FindElement(By.XPath(gotomenu("CurrenciesSeachbutton"))).Click();
                try
                {
                    var element = driver.FindElement(By.XPath(gotomenu("currenciessearchresult")));
                    if (element.Text.Equals("United States Dollars"))
                    {

                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Currencies search  by name','Success','Data Validation',getdate(),'GUI'");
                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Currencies search  by name ','Fail','Data Validation',getdate(),'GUI'");
                    }
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Currencies search by name','Fail','Currency not found exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                }
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Currencies search  by name','Fail','Data Validation exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
            }
            

            // Search by Symbol
            try
            {


                driver.FindElement(By.XPath(gotomenu("currenciessearchbyname"))).Clear();

                var mainfilter = driver.FindElements(By.XPath("//*[@id=\"mainInput\"]")).ToList();
                var element = driver.FindElement(By.XPath("//*[@id=\"page-content-wrapper\"]/div/div/div/vr-panel/div/div/vr-form/div/vr-validation-group/div/vr-row/div/vr-columns[2]/div/vr-textbox"));

                mainfilter[1].SendKeys("USD");
                //  var elements = element.FindElement(By.XPath(gotomenu("currenciessearchbyname")));
                //   elements.SendKeys("USD");
                //  driver.FindElement(By.CssSelector("#mainInput")).SendKeys("USD");

                driver.FindElement(By.XPath(gotomenu("CurrenciesSeachbutton"))).Click();
                try
                {

                    var cur = driver.FindElement(By.XPath("//*[@id=\"rowSection1\"]/div[1]/div[2]/div/div[3]/div/div/div/span"));
                    if (cur.Text.Equals("USD"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Currencies search USD','Success','Currency  found',getdate(),'GUI'");

                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Currencies search USD','Fail','Currency not found',getdate(),'GUI'");
                    }
                }


                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Failed to get Currencies USD','Fail','Fail Seaching Currency exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");


                }
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Failed to get Currencies USD','Fail','Fail Seaching Currency exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
            }
            // add


            try
            {
                driver.FindElement(By.XPath(gotomenu("Currenciesadd"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                 System.Threading.Thread.Sleep(waittime);
                var mainfilter = modaldiag.FindElements(By.XPath("//*[@id=\"mainInput\"]")).ToList();
                 System.Threading.Thread.Sleep(waittime);
                mainfilter[2].SendKeys("Nabil National Currency");
                mainfilter[3].SendKeys("NAB");
                modaldiag.FindElement(By.XPath("Currenciessavebutton")).Click();

                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','save new Currencies','success','success: adding new Currency',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Currencies','Failed to save new Currencies','Fail','Fail adding new Currency exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
            driver.Close();
        }
        // to be written
        public void ratetypes()
        {
                   connect con = new connect();


                   ChromeOptions options = new ChromeOptions();
                   options.AddArgument("--start-maximized");
                   ChromeDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://192.168.110.195:8103");
             System.Threading.Thread.Sleep(waittime);
            // Login
            try
            {
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys("admin@vanrise.com");
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys("1");

                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                 System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login in success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','login in failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                driver.Close();
            }
            // go to look ups 
            try
            {
                driver.FindElement(By.XPath(gotomenu("BusinessEntities"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("Lookups"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("ratetypes"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','navigate to ratetypes','Success','navigation success',getdate(),'GUI'");
                //con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','navigate to Countries','Success','navigation success',getdate(),'GUI'");
            }

            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','navigate to ratetypes','Fail','navigation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                //driver.Close();
            }
            // search if zone exists
            try
            {
                var element = driver.FindElement(By.XPath(gotomenu("ratetypegriditem")));
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','page browsing','Success','browsing success',getdate(),'GUI'");
            //    var element2 = driver.FindElement(By.XPath(gotomenu("codegroupafghcodegroup")));
                string x = element.Text.ToString();
                var count = driver.FindElement(By.XPath(gotomenu("ratetypegridcount")));
                if (count.Text.Equals("Total count (1)"))
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','ratetypes count','Success','data validation success',getdate(),'GUI'");


                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','ratetypes count in correct','Fail','validation failure',getdate(),'GUI'");


                }
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','ratetypes count in correct','Success','navigation success exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
            //search
            driver.FindElement(By.CssSelector("#mainInput")).SendKeys("On-net");
            driver.FindElement(By.CssSelector(gotomenu("ratetypeseachbutton"))).Click();
             System.Threading.Thread.Sleep(waittime);
            var elementg = driver.FindElement(By.XPath(gotomenu("ratetypegriditem")));
             System.Threading.Thread.Sleep(waittime);
            if (elementg.Text.Equals("On-net"))
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','ratetypes search','Success','search success',getdate(),'GUI'");

            }
            else
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','ratetypes search','Fail','search failed no item found',getdate(),'GUI'");
            }

            try
            {
                driver.FindElement(By.CssSelector(gotomenu("ratetypeadd"))).Click();
                var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                var mainfilter = modaldiag.FindElements(By.XPath("//*[@id=\"mainInput\"]")).ToList();
                mainfilter[1].SendKeys("Offnet");
             //   mainfilter[3].SendKeys("NAB");
                modaldiag.FindElement(By.XPath("ratetypesave")).Click();

                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','save new ratetypes','success','success: adding new ratetypes',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'ratetypes','Failed to save new ratetypes','Fail','Fail adding new ratetypes exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
            driver.Close();

        }
        // to be written
        public void carrierprofile()
        {
            waittime = 2000;
            string pagemenu = "Carrierprofiles";
            string parentmenuitem = "";
            string childmenuitem = "";
            string submenuitem = "";
            connect con = new connect();


            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(TextBox1.Text);
            System.Threading.Thread.Sleep(waittime);
            // Login
            try
            {
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys(TextBox2.Text);
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys(TextBox3.Text);

                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login in success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','login in failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                driver.Close();
            }
            // go to look ups 
            try
            {
                DataSet ds1 = con.getdatas("select * from menu where page='" + pagemenu + "'", "Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=no@cce$$dev");
                foreach (DataRow _r in ds1.Tables[0].Rows)
                {
                    parentmenuitem = _r["parent"].ToString();
                    childmenuitem = _r["child"].ToString();
                    submenuitem = _r["sub"].ToString();
                }
                System.Threading.Thread.Sleep(waittime);
                System.Threading.Thread.Sleep(waittime);
                List<IWebElement> mainmenu = driver.FindElements(By.XPath("//span[@class='vr-module-name ng-binding']")).ToList();
                List<IWebElement> submenu = new List<IWebElement>();
                List<IWebElement> childmenu = new List<IWebElement>();
                List<IWebElement> submenus = new List<IWebElement>();
                List<IWebElement> submenusnotnull = new List<IWebElement>();
                List<IWebElement> childmenuss = new List<IWebElement>();
                List<IWebElement> childmenunotnull = new List<IWebElement>();
                List<IWebElement> childmenunotnull2 = new List<IWebElement>();


                try
                {
                    foreach (IWebElement element in mainmenu)
                    {
                        if (element.Text == parentmenuitem)
                        {
                            element.Click();

                            if (childmenuitem != "")
                            {
                                System.Threading.Thread.Sleep(waittime);
                                submenus = driver.FindElements(By.XPath("//div[@class='menulink waves-effect waves-block ng-binding']")).ToList();
                                submenusnotnull = submenus.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                                foreach (IWebElement childelement in submenusnotnull)
                                {
                                    if (childelement.Text == childmenuitem)
                                    {
                                        childelement.Click();
                                        System.Threading.Thread.Sleep(waittime);
                                        childmenuss = driver.FindElements(By.XPath("//a[@class='menulink ng-binding']")).ToList();
                                        childmenunotnull = childmenuss.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                                        foreach (IWebElement subelement in childmenunotnull)
                                        {
                                            if (subelement.Text == submenuitem)
                                            {
                                                subelement.Click();
                                                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrierprofiles','navigate to Carrierprofiles','Success','navigation success',getdate(),'GUI'");
                                            }
                                        }
                                    }

                                }
                            }

                        }
                        else
                        {
                            //  con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','navigate to CodeGroup','Fail','navigation failure exception:failed to go to menu',getdate(),'GUI'");
                        }

                    }
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrierprofiles','navigate to Carrierprofiles','Fail','navigation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                }

                // search if zone exists
                try
                {
                    var element = driver.FindElement(By.XPath(gotomenu("carrierprofilegriditem")));
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','page browsing','Success','browsing success',getdate(),'GUI'");
                    //    var element2 = driver.FindElement(By.XPath(gotomenu("codegroupafghcodegroup")));
                    string x = element.Text.ToString();
                    var count = driver.FindElement(By.XPath(gotomenu("carrierprofilecount")));
                    if (count.Text.Equals("Total count (315)"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile count','Success','data validation success',getdate(),'GUI'");


                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile count in correct','Fail','validation failure',getdate(),'GUI'");


                    }
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile count in correct','Success','validation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }
                //search by name 
                try
                {
                    driver.FindElement(By.CssSelector("#mainInput")).SendKeys("Spactron");
                    driver.FindElement(By.XPath(gotomenu("carrierprofilesearchbutton"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    var elementg = driver.FindElement(By.XPath(gotomenu("carrierprofilenamegriditem")));
                    System.Threading.Thread.Sleep(waittime);
                    if (elementg.Text.Equals("Spactron"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile search','Success','search success',getdate(),'GUI'");

                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile search','Fail','search failed no item found',getdate(),'GUI'");
                    }
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile search','Fail','search failed - page error exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }


                // search by company
                try
                {

                    var profilefilter = driver.FindElements(By.XPath(gotomenu("carrierprofilesearchtextbox"))).ToList();
                    profilefilter[0].Clear();
                    profilefilter[1].SendKeys("Spactron");
                    driver.FindElement(By.XPath(gotomenu("carrierprofilesearchbutton"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    var elementprof = driver.FindElement(By.XPath(gotomenu("carrierprofilenamegriditem")));
                    System.Threading.Thread.Sleep(waittime);
                    if (elementprof.Text.Equals("Spactron"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile search by company','Success','search success',getdate(),'GUI'");

                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile search  by company','Fail','search failed no item found',getdate(),'GUI'");
                    }

                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile search  by company','Fail','search failed - page error exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }
                // search by country

                try
                {

                    var profilefilter = driver.FindElements(By.XPath(gotomenu("carrierprofilesearchtextbox"))).ToList();
                    profilefilter[0].Clear();
                    profilefilter[1].Clear();

                //    driver.FindElement(By.XPath(gotomenu("carrierprofilesearchcomboempty"))).Click();
                  //  System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("carrierprofileselectcountry"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                  //  driver.FindElement(By.XPath(gotomenu("carrierprofilepresscountry"))).Click();
                    driver.FindElement(By.CssSelector("#\\31  > a:nth-child(1) > div:nth-child(1)")).Click();
                    driver.FindElement(By.XPath(gotomenu("carrierprofilesearchbutton"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    var elementprof = driver.FindElement(By.XPath(gotomenu("carrierprofilecountrygriditem")));
                    System.Threading.Thread.Sleep(waittime);
                    if (elementprof.Text.ToLower().Equals("arbinet"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile search by country','Success','search success',getdate(),'GUI'");

                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile search  by country','Fail','search failed no item found',getdate(),'GUI'");
                    }

                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile search  by country','Fail','search failed - page error exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }
                // reset filter start with grid validation

                try
                {
                    var profilefilter = driver.FindElements(By.XPath(gotomenu("carrierprofilesearchtextbox"))).ToList();
                    profilefilter[0].Clear();
                    profilefilter[1].Clear();

                    driver.FindElement(By.XPath(gotomenu("carrierprofilesearchcombo"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("carrierprofilecleardropdown"))).Click();

                    System.Threading.Thread.Sleep(waittime);
                    driver.FindElement(By.XPath(gotomenu("carrierprofilesearchbutton"))).Click();
                    System.Threading.Thread.Sleep(waittime);

                    try
                    {
                        var gridgrids = driver.FindElements(By.XPath(gotomenu("carrierprofilegriddrildownaccount"))).ToList();
                        
                        gridgrids[5].Click();
                        System.Threading.Thread.Sleep(waittime);
                        var gridgrids2 = driver.FindElements(By.CssSelector(gotomenu("carrierprofiledripdowntoproduct"))).ToList();

                        gridgrids2[4].Click();
                        System.Threading.Thread.Sleep(waittime);
                        //  driver.FindElement(By.CssSelector(gotomenu("carrierprofilegriddrildownaccount"))).Click();

                        var grid3 = driver.FindElements(By.CssSelector("#rowSection1 > div:nth-child(1) > div > div > div:nth-child(2) > div > div > div > span")).ToList();
                        var product = grid3[5];
                        if (product.Text.Equals("WholeSale"))
                        {
                            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile assigned product','Success','correct prouct',getdate(),'GUI'");
                        }
                        else
                        {
                            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile assigned product','Fail','wrong prouct',getdate(),'GUI'");
                        }
                        var grid4 = driver.FindElements(By.CssSelector("#rowSection1 > div:nth-child(1) > div > div > div:nth-child(3) > div > div > div > span")).ToList();
                        var productdate = grid4[5];
                        if (productdate.Text.Equals("2016-07-01"))
                        {
                            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile assigned product date','Success','correct prouct date',getdate(),'GUI'");
                        }
                        else
                        {
                            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','carrier profile assigned product date','Fail','wrong prouct date',getdate(),'GUI'");
                        }
                    }
                    catch (Exception Ex)
                    {

                    }
                }
                catch (Exception Ex)
                {

                }

                // Add carrier profile
                try
                {
                    driver.FindElement(By.XPath(gotomenu("carrierprofileadd"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                    var profileelements = modaldiag.FindElements(By.XPath(gotomenu("carrierprofileadditems"))).ToList();
                    profileelements[0].SendKeys("ZTEL");
                    profileelements[1].SendKeys("Zimbabway telecom");
                    profileelements[2].SendKeys("ZTEL");
                    profileelements[3].SendKeys("Zimbabway telecom");
                    profileelements[4].SendKeys("test");
                    profileelements[5].SendKeys("test");
                    profileelements[6].SendKeys("test");
                    profileelements[7].SendKeys("test");
                    profileelements[8].SendKeys("test");
                    profileelements[9].SendKeys("test");



                    modaldiag.FindElement(By.XPath(gotomenu("carrierprofilecontacttab"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    var tab = modaldiag.FindElements(By.XPath(gotomenu("carrierprofileadditems"))).ToList();

                    //tab[25].SendKeys("test@test.com");
                    //tab[24].SendKeys("123123");
                    //tab[23].SendKeys("test@test.com");
                    //tab[22].SendKeys("test");
                    //tab[21].SendKeys("test@test.com");
                    //tab[20].SendKeys("test");
                    //tab[19].SendKeys("test@test.com");
                    //tab[18].SendKeys("test");
                    //tab[17].SendKeys("test@test.com");
                    //tab[16].SendKeys("test");
                    //tab[15].SendKeys("test@test.com");
                    //tab[14].SendKeys("test");
                    //tab[13].SendKeys("test@test.com");
                    //tab[12].SendKeys("test");
                    modaldiag.FindElement(By.XPath(gotomenu("carrierprofilefinincialtab"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    modaldiag.FindElement(By.XPath(gotomenu("carrierprofiletabcurrency"))).Click();
                    System.Threading.Thread.Sleep(waittime);
                    modaldiag.FindElement(By.XPath(gotomenu("carrierprofileselectcurrency"))).Click();
                    var tabs = modaldiag.FindElements(By.XPath("//input[@id='mainInput']")).ToList();

                    //tab[25].SendKeys("test@test.com");
                    //tab[24].SendKeys("123123");
                    //tab[23].SendKeys("test@test.com");
                    //tab[22].SendKeys("test");
                    //tab[21].SendKeys("test@test.com");
                    //tab[20].SendKeys("test");
                    tabs[24].SendKeys("15");
                    tabs[26].SendKeys("15");
                    tabs[27].SendKeys("15");
      
                   // modaldiag.FindElement(By.XPath(gotomenu("carrierprofiledueinput"))).SendKeys("10");
                    modaldiag.FindElement(By.XPath(gotomenu("carrierprofileSave"))).Click();



                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','save new carrier profile','success','success: adding new carrier profile',getdate(),'GUI'");
                }
                catch (Exception Ex)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','Failed to save new carrier profile','Fail','Fail adding new carrier profile exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

                }
                driver.Close();
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier profile','Failed to save new carrier profile','Fail','Fail adding new carrier profile exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
        }
        // to be written
        public void carrieraccount()
        {
            connect con = new connect();

            
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://192.168.110.195:8103");
           
             System.Threading.Thread.Sleep(waittime);
            // Login
            try
            {
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys("admin@vanrise.com");
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys("1");

                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                 System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login in success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','login in failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                driver.Close();
            }
            // go to look ups 
            try
            {
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("BusinessEntities"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("Carriers"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrieraccount"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','navigate to carrier account','Success','navigation success',getdate(),'GUI'");
                //con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','navigate to Countries','Success','navigation success',getdate(),'GUI'");
            }

            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','navigate to carrier account','Fail','navigation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                //driver.Close();
            }
            // search if zone exists
            try
            {
                var element = driver.FindElement(By.XPath(gotomenu("carrierprofilegriditem")));
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','page browsing','Success','browsing success',getdate(),'GUI'");
                //    var element2 = driver.FindElement(By.XPath(gotomenu("codegroupafghcodegroup")));
                string x = element.Text.ToString();
                var count = driver.FindElement(By.XPath(gotomenu("carrieraccountcount")));
                if (count.Text.Equals("Total count (34)"))
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account count','Success','data validation success',getdate(),'GUI'");


                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account count in correct','Fail','validation failure',getdate(),'GUI'");


                }
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account count in correct','Fail','validation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
            //search by name 
            try
            {
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys("spactron");
                driver.FindElement(By.CssSelector(gotomenu("carrieraccountsearchbutton"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                var elementg = driver.FindElement(By.XPath(gotomenu("carrieraccountnamegriditem")));
                 System.Threading.Thread.Sleep(waittime);
                if (elementg.Text.ToLower().Equals("spactron"))
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search','Success','search success',getdate(),'GUI'");

                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search','Fail','search failed no item found',getdate(),'GUI'");
                }
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search','Fail','search failed - page error exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }


            // search by Profile
            try
            {

                var profilefilter = driver.FindElements(By.XPath(gotomenu("carrierprofilesearchtextbox"))).ToList();
                profilefilter[0].Clear();
                driver.FindElement(By.XPath(gotomenu("carrieraccountsearchcombo"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrieraccountselectprofile"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.CssSelector(gotomenu("carrieraccountsearchbutton"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                var elementprof = driver.FindElement(By.XPath(gotomenu("carrieraccountnamegriditem")));
                 System.Threading.Thread.Sleep(waittime);
                if (elementprof.Text.ToLower().Equals("arbinet"))
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search by profile','Success','search success',getdate(),'GUI'");

                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search  by profile','Fail','search failed no item found',getdate(),'GUI'");
                }

            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search  by profile','Fail','search failed - page error exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
            // search by country

            try
            {

                var profilefilter = driver.FindElements(By.XPath(gotomenu("carrierprofilesearchtextbox"))).ToList();
                profilefilter[0].Clear();
               // profilefilter[1].Clear();
                driver.FindElement(By.XPath(gotomenu("carrieraccountsearchcombo"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrieraccountclearprofile"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrieraccountstatuscombo"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrieraccountactivedropdown"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.CssSelector(gotomenu("carrieraccountsearchbutton"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                var elementprof = driver.FindElement(By.XPath(gotomenu("carrieraccountnamegriditem")));
                 System.Threading.Thread.Sleep(waittime);
                if (elementprof.Text.Equals("Blocked Account"))
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search by status','Success','search success',getdate(),'GUI'");

                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search  by status','Fail','search failed no item found',getdate(),'GUI'");
                }

            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search  by status','Fail','search failed - page error exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
          
            try
            {

                var profilefilter = driver.FindElements(By.XPath(gotomenu("carrierprofilesearchtextbox"))).ToList();
                profilefilter[0].Clear();
                // profilefilter[1].Clear();
                //driver.FindElement(By.XPath(gotomenu("carrieraccountsearchcombo"))).Click();
                //System.Threading.Thread.Sleep(1000);
                //driver.FindElement(By.XPath(gotomenu("carrieraccountclearprofile"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrieraccountstatuscombo"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrieraccountclearstatus"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrieraccounttype"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrieraccountexchangetype"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.CssSelector(gotomenu("carrieraccountsearchbutton"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                var elementprof = driver.FindElement(By.XPath(gotomenu("carrieraccountnamegriditem")));
                 System.Threading.Thread.Sleep(waittime);
                if (elementprof.Text.Equals("VoiceKings"))
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search by type','Success','search success',getdate(),'GUI'");

                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search  by type','Fail','search failed no item found',getdate(),'GUI'");
                }

            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account search  by type','Fail','search failed - page error exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
            // reset filter start with grid validation

            try
            {
                var profilefilter = driver.FindElements(By.XPath(gotomenu("carrierprofilesearchtextbox"))).ToList();
                profilefilter[0].Clear();
                profilefilter[1].Clear();

                driver.FindElement(By.XPath(gotomenu("carrierprofilesearchcombo"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("carrierprofilecleardropdown"))).Click();

                 System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.CssSelector(gotomenu("carrierprofilesearchbutton"))).Click();
                 System.Threading.Thread.Sleep(waittime);

                try
                {
                    var gridgrids = driver.FindElements(By.CssSelector(gotomenu("carrierprofilegriddrildownaccount"))).ToList();
                    gridgrids[3].Click();
                     System.Threading.Thread.Sleep(waittime);
                    var gridgrids2 = driver.FindElements(By.CssSelector(gotomenu("carrierprofilegriddrildownaccount"))).ToList();

                    gridgrids2[4].Click();
                     System.Threading.Thread.Sleep(waittime);
                    //  driver.FindElement(By.CssSelector(gotomenu("carrierprofilegriddrildownaccount"))).Click();

                    var grid3 = driver.FindElements(By.CssSelector("#rowSection1 > div:nth-child(1) > div > div > div:nth-child(2) > div > div > div > span")).ToList();
                    var product = grid3[5];
                    if (product.Text.Equals("WholeSale"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account assigned product','Success','correct prouct',getdate(),'GUI'");
                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account assigned product','Fail','wrong prouct',getdate(),'GUI'");
                    }
                    var grid4 = driver.FindElements(By.CssSelector("#rowSection1 > div:nth-child(1) > div > div > div:nth-child(3) > div > div > div > span")).ToList();
                    var productdate = grid4[5];
                    if (productdate.Text.Equals("2016-07-01"))
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account assigned product date','Success','correct prouct date',getdate(),'GUI'");
                    }
                    else
                    {
                        con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','carrier account assigned product date','Fail','wrong prouct date',getdate(),'GUI'");
                    }
                }
                catch (Exception Ex)
                {

                }
            }
            catch (Exception Ex)
            {

            }

            // Add carrier profile
            try
            {
                driver.FindElement(By.XPath(gotomenu("carrierprofileadd"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                var profileelements = modaldiag.FindElements(By.XPath(gotomenu("carrierprofileadditems"))).ToList();
                profileelements[0].SendKeys("ZTEL");
                profileelements[1].SendKeys("Zimbabway telecom");
                profileelements[2].SendKeys("ZTEL");
                profileelements[3].SendKeys("Zimbabway telecom");
                profileelements[4].SendKeys("test");
                profileelements[5].SendKeys("test");
                profileelements[6].SendKeys("test");
                profileelements[7].SendKeys("test");
                profileelements[8].SendKeys("test");
                profileelements[9].SendKeys("test");



                modaldiag.FindElement(By.XPath(gotomenu("carrierprofileaddtab"))).Click();
                 System.Threading.Thread.Sleep(waittime);
                var tab = modaldiag.FindElements(By.XPath(gotomenu("carrierprofileadditems"))).ToList();

                tab[25].SendKeys("test@test.com");
                tab[24].SendKeys("123123");
                tab[23].SendKeys("test@test.com");
                tab[22].SendKeys("test");
                tab[21].SendKeys("test@test.com");
                tab[20].SendKeys("test");
                tab[19].SendKeys("test@test.com");
                tab[18].SendKeys("test");
                tab[17].SendKeys("test@test.com");
                tab[16].SendKeys("test");
                tab[15].SendKeys("test@test.com");
                tab[14].SendKeys("test");
                tab[13].SendKeys("test@test.com");
                tab[12].SendKeys("test");

                modaldiag.FindElement(By.XPath(gotomenu("carrierprofileSave"))).Click();



                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','save new carrier account','success','success: adding new carrier account',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'carrier account','Failed to save new carrier account','Fail','Fail adding new carrier account exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
            driver.Close();

        }
        // to be written
        public void switches()
          {

          }
        // to be written
        public void switchconnect()
        {

        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            UnitTest1 t = new UnitTest1();
            string cons = "";
            connect con = new connect();
            DataSet dprc = con.getdata("select * from testcasedate where testcase='testcase51'",cons);
            foreach (DataRow _r in dprc.Tables[0].Rows)
            {
                t.process_pricelist_testcase(_r["testcase"].ToString(), _r["description"].ToString(),int.Parse(_r["pricelisttype"].ToString()));
            }
            bindgrid();
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriver driver = new ChromeDriver(options);
            //string x = "";
            driver.Navigate().GoToUrl("http://192.168.110.195:8103");
            System.Threading.Thread.Sleep(1000);
            driver.FindElement(By.CssSelector("#mainInput")).SendKeys("admin@vanrise.com");
            driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys("1");
            driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
            System.Threading.Thread.Sleep(6000);
            List<IWebElement> mainmenu = driver.FindElements(By.XPath("//span[@class='vr-module-name ng-binding']")).ToList();
            List<IWebElement> submenu=new List<IWebElement>();
            List<IWebElement> childmenu = new List<IWebElement>();
            List<IWebElement> submenus = new List<IWebElement>();
            List<IWebElement> submenusnotnull = new List<IWebElement>();
            List<IWebElement> childmenuss = new List<IWebElement>() ;
            List<IWebElement> childmenunotnull = new List<IWebElement>();
            List<IWebElement> childmenunotnull2 = new List<IWebElement>();
           
            string parentmenuitem = "";
            string childmenuitem = "";
            string submenuitem = "";
            string pagemenuitem = "";
            connect con = new connect();
            con.updatedata("delete from menu");
            foreach (IWebElement element in mainmenu)
            {
                parentmenuitem = element.Text.ToString();
                try
                {
                    element.Click();
                    try
                    {
                        System.Threading.Thread.Sleep(1000);
                        submenus = driver.FindElements(By.XPath("//div[@class='menulink waves-effect waves-block ng-binding']")).ToList();
                        submenusnotnull = submenus.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                    
                    }
                    catch { }

                    try
                    {
                        System.Threading.Thread.Sleep(1000);
                        childmenuss = driver.FindElements(By.XPath("//a[@class='menulink  ng-binding']")).ToList();
                        childmenunotnull = childmenuss.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                        System.Threading.Thread.Sleep(1000);
                        if (childmenunotnull.Count() > 0)
                        {
                            childmenuitem = "";
                            foreach (IWebElement ch in childmenunotnull)
                            {
                                submenuitem = ch.Text.ToString();
                                con.updatedata("INSERT INTO [dbo].[menu]([parent],[child],[sub],[page]) VALUES ('" + parentmenuitem + "' ,'" + childmenuitem + "','" + submenuitem + "','" + submenuitem.Replace(" ","") + "')");
                            }

                          
                           // ProcessGUI(childmenuss);
                        }
                    }
                    catch { }

                    foreach (IWebElement subelemet in submenusnotnull)
                    {
                        try
                        {
                            subelemet.Click();
                            childmenuitem = subelemet.Text.ToString();
                            List<IWebElement> childmenus = driver.FindElements(By.XPath("//a[@class='menulink ng-binding']")).ToList();
                            childmenunotnull2 = childmenus.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                            System.Threading.Thread.Sleep(1000);
                            if (childmenunotnull2.Count() > 0)
                            {
                                System.Threading.Thread.Sleep(1000);
                        
                                foreach (IWebElement El in childmenunotnull2)
                                {
                                   // ProcessGUI(El);
                                    submenuitem = El.Text.ToString();
                                    con.updatedata("INSERT INTO [dbo].[menu]([parent],[child],[sub],[page]) VALUES ('" + parentmenuitem + "' ,'" + childmenuitem + "','" + submenuitem + "','" + submenuitem.Replace(" ", "") + "')");
                                }
                            }
                        }
                        catch
                        {

                        }

                    }
                }
                catch
                {

                }
            }

            
            System.Threading.Thread.Sleep(1000);

            List<IWebElement> submenufinal = new List<IWebElement>();
            List<IWebElement> childmenufinal = new List<IWebElement>();
            submenufinal = submenu.Where(x =>!string.IsNullOrEmpty(x.Text)).ToList();
            childmenufinal = childmenu.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
            driver.Close();
        }

        private void ProcessGUI(IWebElement El)
        {
            El.Click();
            if (El.Text == "Code Groups")
            {
                codegroups();
            }
            if (El.Text == "Countries")
            {
                GUIcountry();
            }
            if (El.Text == "Switches")
            {
                switches();
            }
            if (El.Text == "Carrier Accounts")
            {
                carrieraccount();
            }
            if (El.Text == "Carrier Profiles")
            {
                carrierprofile();
            }
            if (El.Text == "Cities")
            {
                GUICities();
            }
            //if (El.Text == "")
            //{

            //}
            //if (El.Text == "")
            //{

            //}
            //if (El.Text == "")
            //{

            //}
            //if (El.Text == "")
            //{

            //}

            throw new NotImplementedException();
        }

        public void codegroups()
        {
            connect con = new connect();


            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(TextBox1.Text);
            System.Threading.Thread.Sleep(waittime);
            // Login
            try
            {
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys(TextBox2.Text);
                driver.FindElement(By.XPath("//input[@placeholder=\"Password\"]")).SendKeys(TextBox3.Text);

                driver.FindElement(By.CssSelector("body > div > div > div > div > div > vr-form > div > vr-validation-group > div:nth-child(3) > div > vr-button > div > button")).Click();
                System.Threading.Thread.Sleep(waittime);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','login in success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','login in failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                driver.Close();
            }
            // go to look ups 
            try
            {
                //driver.FindElement(By.XPath(gotomenu("BusinessEntities"))).Click();
                // System.Threading.Thread.Sleep(waittime);
                //driver.FindElement(By.XPath(gotomenu("Lookups"))).Click();
                // System.Threading.Thread.Sleep(waittime);
                //driver.FindElement(By.XPath(gotomenu("Codegroups"))).Click();
                // System.Threading.Thread.Sleep(waittime);

                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','navigate to CodeGroup','Success','navigation success',getdate(),'GUI'");
                //con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','navigate to Countries','Success','navigation success',getdate(),'GUI'");
            }

            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','navigate to CodeGroup','Fail','navigation failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                //driver.Close();
            }
            // search if zone exists
            try
            {
                var element = driver.FindElement(By.XPath(gotomenu("codegroupgriditem")));
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','page browsing','Success','browsing success',getdate(),'GUI'");
                var element2 = driver.FindElement(By.XPath(gotomenu("codegroupafghcodegroup")));
                string x = element.Text.ToString();
                var count = driver.FindElement(By.XPath(gotomenu("codegroupgridcount")));
                if (count.Text.Equals("Total count (270)"))
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup count','Success','navigation success',getdate(),'GUI'");


                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup count in correct','Success','navigation success',getdate(),'GUI'");


                }
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup count in correct','Success','navigation success exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");

            }
            try
            {
                // search for a specific zone
                driver.FindElement(By.CssSelector("#mainInput")).SendKeys("961");
                System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.CssSelector(gotomenu("codegroupsearchitem"))).Click();
                System.Threading.Thread.Sleep(waittime);
                var elementg = driver.FindElement(By.XPath(gotomenu("codegroupgetitem")));
                System.Threading.Thread.Sleep(waittime);
                if (elementg.Text.Equals("Lebanon"))
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup search','Success','search success',getdate(),'GUI'");

                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup search','Fail','search failed no item found',getdate(),'GUI'");
                }
                //  string currentHandle = driver.CurrentWindowHandle;
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup search','Fail','search failed no item found exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
            }
            try
            {
                driver.FindElement(By.XPath(gotomenu("codegroupaddpopup"))).Click();
                System.Threading.Thread.Sleep(waittime);

                // add a new zone
                var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                System.Threading.Thread.Sleep(waittime);
                var textinput = modaldiag.FindElement(By.CssSelector("#mainInput"));
                textinput.SendKeys("93222");
                System.Threading.Thread.Sleep(waittime);
                var textsave = modaldiag.FindElement(By.CssSelector(gotomenu("codegroupaddcountrydropdown")));
                textsave.Click();
                // driver.FindElement(By.XPath("[@class,'modal-dialog'][@id='mainInput']")).SendKeys("Calgary");
                System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("codegroupaddcountryclickcontrol"))).Click();
                System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("codegroupaddsavebutton"))).Click();
                // driver.Close();
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup add','Success','adding CodeGroup success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup add','Fail','adding CodeGroup failure exception:" + Ex.ToString().Replace('"', ' ') + "',getdate(),'GUI'");
                //  driver.Close();
            }

            try
            {
                //edit code group

                driver.FindElement(By.XPath(gotomenu("codegroupeditpressgrid"))).Click();

                System.Threading.Thread.Sleep(waittime);
                driver.FindElement(By.XPath(gotomenu("codegrouoeditpopup"))).Click();
                System.Threading.Thread.Sleep(waittime);
                var modaldiag = driver.FindElement(By.ClassName("modal-dialog"));
                System.Threading.Thread.Sleep(waittime);
                var textinput = modaldiag.FindElement(By.CssSelector("#mainInput"));
                System.Threading.Thread.Sleep(waittime);
                var savebutton = modaldiag.FindElement(By.XPath(gotomenu("codegroupeditpopupsave")));
                System.Threading.Thread.Sleep(waittime);
                savebutton.Click();
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup add','Success','adding CodeGroup Success',getdate(),'GUI'");
            }
            catch (Exception Ex)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','CodeGroup Edit','Fail','Editing CodeGroup failure exception:" + Ex.ToString().Replace('"', ' ').Replace('"', ' ') + "',getdate(),'GUI'");
            }
            driver.Close();


        }

        private void findpage(string p, ChromeDriver driver)
        {
            System.Threading.Thread.Sleep(4000);
        

            List<IWebElement> mainmenu = driver.FindElements(By.XPath("//span[@class='vr-module-name ng-binding']")).ToList();
            List<IWebElement> submenu = new List<IWebElement>();
            List<IWebElement> childmenu = new List<IWebElement>();
            List<IWebElement> submenus = new List<IWebElement>();
            List<IWebElement> submenusnotnull = new List<IWebElement>();
            List<IWebElement> childmenuss = new List<IWebElement>();
            List<IWebElement> childmenunotnull = new List<IWebElement>();
            List<IWebElement> childmenunotnull2 = new List<IWebElement>();
            System.Threading.Thread.Sleep(1000);

            foreach (IWebElement element in mainmenu)
            {
                try
                {
                    element.Click();
                    try
                    {
                        System.Threading.Thread.Sleep(1000);
                        submenus = driver.FindElements(By.XPath("//div[@class='menulink waves-effect waves-block ng-binding']")).ToList();
                        System.Threading.Thread.Sleep(1000);
                        submenusnotnull = submenus.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                        if (submenusnotnull.Count() > 0)
                        {
                            submenu.AddRange(submenusnotnull);
                        }
                    }
                    catch { }

                    try
                    {
                        System.Threading.Thread.Sleep(1000);
                        childmenuss = driver.FindElements(By.XPath("//div[@class='menulink ng-binding']")).ToList();
                        childmenunotnull = childmenuss.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                        System.Threading.Thread.Sleep(1000);
                        if (childmenunotnull.Count() > 0)
                        {
                            childmenu.AddRange(childmenunotnull);
                            // ProcessGUI(childmenuss);
                        }
                    }
                    catch { }

                    foreach (IWebElement subelemet in submenusnotnull)
                    {
                        try
                        {

                            subelemet.Click();
                            System.Threading.Thread.Sleep(1000);
                            List<IWebElement> childmenus = driver.FindElements(By.XPath("//a[@class='menulink ng-binding']")).ToList();
                            childmenunotnull2 = childmenus.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
                            System.Threading.Thread.Sleep(1000);
                            if (childmenunotnull2.Count() > 0)
                            {
                                System.Threading.Thread.Sleep(1000);
                                childmenu.AddRange(childmenunotnull2);
                                foreach (IWebElement El in childmenunotnull2)
                                {
                                    if(El.Text==p)
                                    {
                                        El.Click();
                                        return;
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }

                    }
                }
                catch
                {

                }
            }

            throw new NotImplementedException();
        }
       

        private void gotopagemenu(ChromeDriver driver, string parent, string child, string sub)
        {


        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            string x = "";
            x = DropDownList1.SelectedItem.Value.ToString();
            if(x.ToLower().Equals("login"))
            {
                login();   //Done
            }
            if (x.ToLower().Equals("Countries"))
            {
                //  GUIcountry();  //done
            }
            if (x.ToLower().Equals("cities"))
            {
                // GUICities();     //done
            }
            if (x.ToLower().Equals("carrieraccounts"))
            {
                carrieraccount();
            }
            if (x.ToLower().Equals("carrierprofiles"))
            {
                carrierprofile();
            }
            if (x.ToLower().Equals("codegroups"))
            {
                // codegroup();  //done
            }
            if (x.ToLower().Equals("switches"))
            {
               
                                          
                //  carrieraccount();
                //   currencies();
                //   ratetypes();
                //  
                // 

            }
            bindgrid();
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            DateTime? d;
          
           // d = string.IsNullOrEmpty(x.Trim('"')) ? default(DateTime?) :DateTime.ParseExact(x.Trim('"'), "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture);
          

        }

        protected void Button7_Click(object sender, EventArgs e)
        {

            TextBox11.Text = Math.Round(Decimal.Parse(TextBox11.Text), 2, MidpointRounding.AwayFromZero).ToString();
        }


    
    }
    }