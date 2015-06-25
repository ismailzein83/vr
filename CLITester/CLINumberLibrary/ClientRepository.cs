using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLINumberLibrary
{
    public class ClientRepository
    {
        public static Client Load(int clientId)
        {
            Client client = new Client();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    client = context.Clients.Where(l => l.Id == clientId).FirstOrDefault<Client>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return client;
        }

        public static Client Load(string name, string password)
        {
            Client client = new Client();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    client = context.Clients.Where(l => l.Name == name && l.Password == password).FirstOrDefault<Client>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return client;
        }
    }
}
