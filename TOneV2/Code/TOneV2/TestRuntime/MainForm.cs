using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestRuntime
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach(var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if(t != typeof(ITask) && typeof(ITask).IsAssignableFrom(t))
                {
                    cmbTask.Items.Add(t);
                }
            }
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Type selectedTaskType = cmbTask.SelectedItem as Type;
            ITask task = Activator.CreateInstance(selectedTaskType) as ITask;
            Task t = new Task(() => task.Execute());
            t.Start();
            this.Close();
        }
    }
}
