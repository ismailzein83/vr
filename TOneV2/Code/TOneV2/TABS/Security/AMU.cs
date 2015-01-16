using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS.Security
{
    public class AMU
    {
        #region "Properties"
        public int ID { get; set; }
        public SecurityEssentials.User User { get; set; }
        public string Flag { get; set; }
        public bool IsExecutive { get; set; }
        public int Level { get; set; }
        public bool IsEmpty { get; set; }
        public AMU Manager { get; set; }
        public List<AMU> Managed_AMUs { get; set; }

        public List<AMU_Customer> DirectCustomers { get; set; }

        public static List<AMU_Customer> InDirectCustomers { get; set; }

        public static List<AMU_Customer> Customers { get; set; }

        #endregion "Properties"

        #region "Methods"

        public void AssignCarrier(CarrierAccount carrier, MCT mct) { }

        public void AssignManaged_AMU(User user) { }

        public void UnAssignCarrier(CarrierAccount carrier) { }

        public void UnAssignManaged_AMU(User user) { }

        public void UnAssignCarriersFromManaged_AMUs(List<CarrierAccount> CarriersToRemove) { }

        public void CheckValidMct(MCT mct) { }

        public List<AMU> GetManagementTree()
        {
            return new List<AMU>();
        }

        public List<AMU> GetManagedTree()
        {
            return new List<AMU>();
        }

        private string AssignFlag()
        {
            string flag = string.Empty;
            if (Manager != null)
            {
                string managerFlag = Manager.Flag;
                string userID = User.ID < 10 ? "0" + User.ID.ToString() : User.ID.ToString();
                flag = managerFlag + userID;
            }
            else
            {
                flag = User.ID < 10 ? "0" + User.ID.ToString() : User.ID.ToString();
            }
            return flag;
        }

        private List<AMU_Customer> GetIndirectCustomers()
        {
            List<AMU_Customer> indirectCustomers = new List<AMU_Customer>();
            foreach (var amu in GetManagedTree())
            {
                foreach (var customer in amu.DirectCustomers)
                    indirectCustomers.Add(customer);
            }
            return indirectCustomers;
        }

        #endregion "Methods"

        #region "OldVersion"

        public static string DefaultXml
        {
            get
            {
                return @"<Accounts>
                           <Customers>
                              <CarrierAccount>$CarrierAccountID$,$Date$,$MCT$</CarrierAccount>
                           </Customers>
                           <SubAccountManagers>
                              <AccountManager>-1</AccountManager>
                           </SubAccountManagers>
                           <PricingTemplate>-1</PricingTemplate> 
                           <Manager>-1</Manager>
                           <Level>-1</Level>
                           <ActionType>-1</ActionType>
                         </Accounts>";
            }
        }



        #endregion "OldVersion"


    }

    public class MCT
    {
        public int ID { get; set; }
        public static MCT Get(int ID)
        {
            return ID <= 0 ? new MCT { ID=3 } : Mcts.FirstOrDefault(c => c.ID == ID);
        }

        private static MCT[] Mcts = new MCT[] 
        {
            new MCT { ID = 1 },
            new MCT { ID = 2 },
            new MCT { ID = 3 }
        };
    }

    //public class AMU_Customer
    //{
    //    public CarrierAccount Customer { get; set; }
    //    public MCT MCT { get; set; }
    //    public AMU Direct_AMU { get; set; }
    //    public DateTime AssignDate { get; set; }
    //}
}
