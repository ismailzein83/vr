using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;

namespace TABS.AutomaticInvoiceReports
{
    public class BillingInvoiceGenerator
    {
        public enum ReportType
        {
            NONE = 0,
            RPTENVOICE = 1,
            RPTGROUPEDINVOICE = 2

        };
        protected static string GetClassContent(string customCode, out string ClassName)
        {

            ClassName = "CustomBillingInvoiceGenerator_" + Math.Abs(customCode.GetHashCode());
            string ifsts = " ";
            string CloseCns = "  ";
            StringBuilder BillingInvoiceFunc = new StringBuilder();
            BillingInvoiceFunc.Append(@"public Telerik.Reporting.IReportDocument GetBillingInvoice(TABS.Billing_Invoice Invoice)
                        {").Append("TABS.AutomaticInvoiceReports.").Append(ClassName).Append(@"  
                               report = new ").Append("TABS.AutomaticInvoiceReports.").Append(ClassName).Append(@"(Invoice);
                             
                            
                            return (Telerik.Reporting.IReportDocument)report;
                        }");

            if (!customCode.Contains("GetBillingInvoice"))
            {
                ifsts = "InitializeComponent(); this.InvoiceDetails.ReportSource.DataSource = Invoice.Billing_Invoice_Details;((TABS.AutomaticInvoiceReports.RptInVoiceDetail)this.InvoiceDetails.ReportSource).BillingInvoice = Invoice;DisplayHeader();if (Invoice.Customer.CarrierProfile.VAT == 0) this.VatPanel.Parent.Items.Remove(this.VatPanel);} public TABS.CarrierAccount Customer { get { return this.BillingInvoice.Customer; } }  public Telerik.Reporting.SubReport InvoiceDetails{get { return this.SRinvoiceDetail; } }   ";//public  Telerik.Reporting.Panel VatPanel{get{return this.VatPanel;}}
                customCode = BillingInvoiceFunc.ToString() + "  " + customCode;
                CloseCns = " ";
            }
            else CloseCns = "}  ";
            string @ClassDefinition = (new StringBuilder()).Append(@"
            
                    using System;
                    using System.ComponentModel;
                    using System.Drawing;
                    using System.Linq;
                    using System.Windows.Forms;
                    using Telerik.Reporting;
                    using Telerik.Reporting.Drawing;
                    using System.Collections.Generic;
                    using TABS;
                    namespace TABS
                   {
                    public class ").Append(ClassName).Append(" : ").Append(typeof(Telerik.Reporting.Report).FullName).Append(",TABS.AutomaticInvoiceReports.IBillingInvoiceGenerator").Append(@"
                    {
                        private TABS.Billing_Invoice _Invoice;
                        public TABS.Billing_Invoice BillingInvoice { get; set; }
                        public  ").Append(ClassName).Append(@"(TABS.Billing_Invoice Invoice)
                        {
                            this.BillingInvoice = Invoice;
                           
                            ").Append(ifsts).Append(CloseCns).Append(@"
                            
                        
                         
                        ").Append(customCode).Append(@"
                    }
                }
                ").ToString();

            return ClassDefinition;
        }
        protected static string GetClassContentGroubed(string customCode, out string ClassName)
        {

            ClassName = "CustomBillingInvoiceGenerator_" + Math.Abs(customCode.GetHashCode());
            string ifsts = " ";
            string CloseCns = "  ";
            StringBuilder BillingInvoiceFunc = new StringBuilder();
            BillingInvoiceFunc.Append(@"public Telerik.Reporting.IReportDocument GetBillingInvoice(TABS.Billing_Invoice Invoice)
                        {").Append("TABS.AutomaticInvoiceReports.").Append(ClassName).Append(@"  
                               report = new ").Append("TABS.AutomaticInvoiceReports.").Append(ClassName).Append(@"(Invoice);
                             
                            
                            return (Telerik.Reporting.IReportDocument)report;
                        }");

            if (!customCode.Contains("GetBillingInvoice"))
            {
                ifsts = "InitializeComponent(); this.InvoiceDetails.ReportSource.DataSource = Invoice.Billing_Invoice_Details;((TABS.AutomaticInvoiceReports.RptGroupedInVoiceDetail)this.InvoiceDetails.ReportSource).BillingInvoice = Invoice;DisplayHeader();if (Invoice.Customer.CarrierProfile.VAT == 0) this.VatPanel.Parent.Items.Remove(this.VatPanel);} public TABS.CarrierAccount Customer { get { return this.BillingInvoice.Customer; } }  public Telerik.Reporting.SubReport InvoiceDetails{get { return this.SRinvoiceDetail; } }  ";
                customCode = BillingInvoiceFunc.ToString() + "  " + customCode;
                CloseCns = " ";
            }
            else CloseCns = "}";
            string @ClassDefinition = (new StringBuilder()).Append(@"
            
                    using System;
                    using System.ComponentModel;
                    using System.Drawing;
                    using System.Windows.Forms;
                    using Telerik.Reporting;
                    using Telerik.Reporting.Drawing;
                    using System.Collections.Generic;
                    using System.Collections;
                    using System.Linq;
                    using TABS;
                    namespace TABS
                   {
                    public class ").Append(ClassName).Append(" : ").Append(typeof(Telerik.Reporting.Report).FullName).Append(",TABS.AutomaticInvoiceReports.IBillingInvoiceGenerator").Append(@"
                    {
                        private TABS.Billing_Invoice _Invoice;
                       
                        public  ").Append(ClassName).Append(@"(TABS.Billing_Invoice Invoice)
                        {
                            this.BillingInvoice = Invoice;
                            
                            ").Append(ifsts).Append(CloseCns).Append(@"
                            
                       
                         public TABS.Billing_Invoice BillingInvoice { get; set; }
                        
                        ").Append(customCode).Append(@"
                    }
                }
                ").ToString();

            return ClassDefinition;
        }

        public static Telerik.Reporting.IReportDocument GetBillingInvoiceWorkSheet(string customCode, TABS.Billing_Invoice Invoice, int type)
        {
            //TABS.Reports.RptInvoice report = new RptInvoice(Invoice);


            Telerik.Reporting.IReportDocument r = null;


            string className = null;

            string classDefinition = (type == 1) ? GetClassContent(customCode, out className) : GetClassContentGroubed(customCode, out className);

            try
            {
                Dictionary<string, string> providerOptions = new Dictionary<string, string>();
                providerOptions["CompilerVersion"] = "v3.5";
                Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);

                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateExecutable = false;
                parameters.GenerateInMemory = true;
                parameters.IncludeDebugInformation = true;

                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Data.dll");
                parameters.ReferencedAssemblies.Add("System.Web.dll");
                parameters.ReferencedAssemblies.Add(typeof(System.Linq.Enumerable).Assembly.Location);
                parameters.ReferencedAssemblies.Add("System.Drawing.dll");
                parameters.ReferencedAssemblies.Add("System.Xml.dll");

                parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                string binPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");
                binPath = binPath + @"\NHibernate.dll";
                parameters.ReferencedAssemblies.Add(binPath);
                // parameters.ReferencedAssemblies.Add(binPath);//"Telerik.Reporting.dll"

                parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
                parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

                string refDir = Addons.AddonManager.AddonsLocation + "\\";


                foreach (AssemblyName referenced in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {

                    //currentAssembly=Assembly.GetExecutingAssembly().GetReferencedAssemblies().ElementAt(cuurentAssembly);
                    string test = referenced.CodeBase;
                    System.IO.FileInfo info = new System.IO.FileInfo(refDir + referenced.Name + ".dll");
                    if (info.Exists)
                    {
                        if (!parameters.ReferencedAssemblies.Contains(info.FullName))
                            parameters.ReferencedAssemblies.Add(info.FullName);
                        //if (referenced.Name == "TABS")
                        //{
                        //    Assembly assembly = Assembly.Load(referenced);
                        //    referencedAssemblies = assembly.GetReferencedAssemblies();
                        //    foreach (AssemblyName a in referencedAssemblies)
                        //        Assembly.Load(a);

                        //}
                    }
                }

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, classDefinition);

                if (results.Errors.Count == 0)
                {
                    //Type generated = results.CompiledAssembly.GetType("TABS.AutomaticInvoiceReports." + className);
                    Type generated = results.CompiledAssembly.GetType("TABS." + className);
                    Type[] types = new Type[1];
                    types[0] = typeof(TABS.Billing_Invoice);
                    object[] MethodParameters = new object[] { Invoice };

                    ConstructorInfo constructorInfoObj = generated.GetConstructor(types);

                    IBillingInvoiceGenerator generator = (IBillingInvoiceGenerator)generated.GetConstructor(types).Invoke(MethodParameters);
                    Telerik.Reporting.IReportDocument report = generator.GetBillingInvoice(Invoice);

                    return report;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(error.ErrorText);
                    }
                    //log.Error(string.Format("ERROR Generating Custom Billing Invoice Generator For:\n{0}", classDefinition), new Exception(sb.ToString()));
                }
            }
            catch (Exception ex)
            {
                //log.Error(string.Format("ERROR Generating Custom Billing Invoice Generator For:\n{0}", classDefinition), ex);
            }

            return r;
        }
    }
}
