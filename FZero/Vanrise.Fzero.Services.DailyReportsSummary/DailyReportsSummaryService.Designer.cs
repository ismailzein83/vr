namespace Vanrise.Fzero.Services.DailyReportsSummary
{
    partial class DailyReportsSummaryService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // DailyReportsSummaryService
            // 
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;

            this.ServiceName = "DailyReportsSummaryService";
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            //this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            //this.serviceProcessInstaller1,
            //this.serviceInstaller1});
        }

        #endregion
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
    }
}
