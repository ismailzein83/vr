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

        public static Client Save(Client client)
        {
            Client currentUser = new Client();
            try
            {
                using (Entities context = new Entities())
                {
                    if (client.ID == 0)
                    {
                        context.Clients.Add(client);
                    }
                    else
                    {
                        context.Clients.Attach(client);
                        context.Entry(client).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Clients.Save(" + client.ID.ToString() + ")", err);
            }
            return currentUser;
        }

        public static Client Load(int ID)
        {
            Client client = new Client();
            try
            {
                using (Entities context = new Entities())
                {
                    client = context.Clients
                     .Where(u => u.ID == ID)
                     .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Client.Load(" + ID.ToString() + ")", err);
            }
            return client;
        }

    }
}
