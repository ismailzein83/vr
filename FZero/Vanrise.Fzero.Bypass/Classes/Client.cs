using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class Client
    {
        public static List<Client> GetAllClients()
        {
            List<Client> ClientsList = new List<Client>();

            try
            {
                using (Entities context = new Entities())
                {
                    ClientsList = context.Clients
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Client.GetClients()", err);
            }

            return ClientsList;
        }
    }
}
