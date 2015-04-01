using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;

namespace OnlineOperatorsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Timer MyTimer = new Timer();
            MyTimer.Interval = (5 * 60 * 1000); // 45 mins
            MyTimer.Tick += new EventHandler(MyTimer_Tick);
            MyTimer.Start();
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            //Call the Android service to get the list of operators
            ServiceReference1.ServiceAuthHeader auth = new ServiceReference1.ServiceAuthHeader();
            ServiceReference1.clireporterSoapClient cc = new ServiceReference1.clireporterSoapClient();
            ServiceReference1.CountryCarrier[] resp;

            auth.Username = "user";
            auth.Password = "idsP@ssw0rdids";

            resp = cc.GetMMCandMNC(auth, "user", "idsP@ssw0rdids");
            
            List<Operator> Operators = OperatorRepository.GetOperators().OrderByDescending(l => l.Id).ToList();
          
            foreach (Operator op in Operators)
            {
                OperatorLog oop = new OperatorLog();
                oop.Id = op.Id;
                oop.Name = op.Name;
                oop.MCC = op.mcc;
                oop.MNC = op.mnc;
                oop.Country = op.Country;
                oop.LogDate = DateTime.Now;
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

                OperatorRepository.InsertLog(oop);
            }
        }
    }
}
