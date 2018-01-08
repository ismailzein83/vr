using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOM.Runtime
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            string filePath = String.Format(@"{0}\selectedTask", Directory.GetCurrentDirectory());


            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t != typeof(ITask) && typeof(ITask).IsAssignableFrom(t))
                {
                    cmbTask.Items.Add(t);
                }
            }
            try
            {
                if (File.Exists(filePath))
                    cmbTask.SelectedItem = Type.GetType(File.ReadAllText(filePath));
            }
            catch
            {
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Type selectedTaskType = cmbTask.SelectedItem as Type;
            string filePath = String.Format(@"{0}\selectedTask", Directory.GetCurrentDirectory());
            File.WriteAllText(filePath, selectedTaskType.FullName);
            ITask task = Activator.CreateInstance(selectedTaskType) as ITask;
            Task t = new Task(() => task.Execute());
            t.ContinueWith((tt) =>
            {
                this.CloseFormAsync();
            });
            t.Start();
            this.WindowState = FormWindowState.Minimized;
        }

        void CloseFormAsync()
        {
            if (this.InvokeRequired)
                this.Invoke(new VoidDelegate(CloseFormAsync));
            else
                this.Close();
        }

        delegate void VoidDelegate();
    }
}
