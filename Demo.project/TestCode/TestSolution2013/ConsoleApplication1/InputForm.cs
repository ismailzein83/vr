using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    public partial class InputForm : Form
    {
        public string InputText
        {
            get
            {
                return textBox1.Text;
            }
        }
        public InputForm()
        {
            InitializeComponent();
        }
    }
}
