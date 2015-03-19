using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroitTest
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class MontyOperator
        {
            public string mcc { get; set; }
            public string mnc { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<MontyOperator> testoperators = new List<MontyOperator>();
            {
                MontyOperator m = new MontyOperator();
                m.mcc = "415";
                m.mnc = "01";
                testoperators.Add(m);

                m = new MontyOperator();
                m.mcc = "418";
                m.mnc = "20";
                testoperators.Add(m);

                m = new MontyOperator();
                m.mcc = "427";
                m.mnc = "01";
                testoperators.Add(m);

                m = new MontyOperator();
                m.mcc = "257";
                m.mnc = "01";
                testoperators.Add(m);

                m = new MontyOperator();
                m.mcc = "427";
                m.mnc = "02";
                testoperators.Add(m);
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"E:\WriteLines2.txt"))
            foreach (MontyOperator op in testoperators)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"E:\WriteLines2.txt"))
                {
                    file.WriteLine(DateTime.Now.ToString());
                    ServiceReference1.ServiceAuthHeader auth = new ServiceReference1.ServiceAuthHeader();
                    ServiceReference1.clireporterSoapClient cc = new ServiceReference1.clireporterSoapClient();
                    ServiceReference1.NumberID resp = new ServiceReference1.NumberID();
                    auth.Username = "user";
                    auth.Password = "idsP@ssw0rdids";
                    string dd = DateTime.Now.Date.Year + "-" + DateTime.Now.Date.Month + "-" + DateTime.Now.Date.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                    resp = cc.RequestForCall(auth, "user", "idsP@ssw0rdids", op.mcc, op.mnc , dd);
                    if (resp.ErrorStatus == "1")
                    {
                        file.WriteLine(DateTime.Now.ToString() + " MCC " + op.mcc + " MNC " + op.mnc + " ErrorStatus " + resp.ErrorStatus);
                    }
                    else
                    {
                        if (resp.mobileNumber != "")
                        {
                            file.WriteLine(DateTime.Now.ToString() + " mobileNumber " + resp.mobileNumber + " MCC " + op.mcc + " MNC " + op.mnc + " ErrorStatus " + resp.ErrorStatus);
                        }
                    }

                }

            }
            label1.Text = "DONE";
        }
    }
}
