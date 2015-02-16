using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.CommonLibrary;



namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class Switch_DatabaseConnections
    {
        public string ConnectionString
        {
            get
            {
                return Constants.ConnectionStringPattern
                    .Replace("@ServerName", this.ServerName)
                    .Replace("@DatabaseName", this.DatabaseName)
                    .Replace("@UserId", this.UserId)
                    .Replace("@UserPassword", this.UserPassword)
                    ;
            }
        }

        public static string GetSwitchConnectionstring(int id)
        {
            Switch_DatabaseConnections switchConnection = LoadConnection(id);
            if (switchConnection != null && switchConnection.Id > 0)
                return switchConnection.ConnectionString;
            return string.Empty;
        }

        public static string GetDatabaseName(int id)
        {
            Switch_DatabaseConnections switchConnection = LoadConnection(id);
            if (switchConnection != null && switchConnection.Id > 0)
                return switchConnection.DatabaseName;
            return string.Empty;
        }

       


        public static Switch_DatabaseConnections LoadConnection(int id)
        {
            Switch_DatabaseConnections SwitchConnection = new Switch_DatabaseConnections();
            try
            {
                using (Entities context = new Entities())
                {
                    SwitchConnection = context.Switch_DatabaseConnections.Where(u=>u.Id==id).ToList().FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Switch_DatabaseConnections.LoadConnection(" + id + ")", err);
            }
            return SwitchConnection;
        }







    }
}
