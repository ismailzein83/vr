using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TOne.WhS.DBSync.App
{
    static class Program
    {
        #region Configuration Parameters
        public static string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

        #endregion
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MigrationForm());
        }
    }
}
