using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QualityMeasurement.DevRuntime
{
    public partial class Form1 : Form
    {
        private readonly GetCalls _thGetCalls = new GetCalls();
        private readonly GetResults _thGetResults = new GetResults();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _thGetCalls.Start(this);
            _thGetResults.Start(this);
        }
    }
}
