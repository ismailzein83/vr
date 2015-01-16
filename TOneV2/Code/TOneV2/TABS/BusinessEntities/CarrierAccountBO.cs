using System.Data;

namespace TABS.BusinessEntities
{
    public class CarrierAccountBO
    {
        public static DataTable GetCarrierAccountIDs()
        {
            return TABS.DataHelper.GetDataTable("SELECT CarrierAccountID FROM CarrierAccount");
        }

        public static void DropAndCreateCarrierSwitchMapping()
        {
            TABS.DataHelper.ExecuteNonQuery("DROP TABLE SwitchCarrierMapping; CREATE TABLE SwitchCarrierMapping(SwitchID TINYINT, CarrierAccountID VARCHAR(10), Identifier varchar(100), IsIn char(1), IsOut char(1))");
        }

        public static void InsertSwitchCarrierMappings()
        {
            string sql = "INSERT INTO SwitchCarrierMapping(SwitchID, CarrierAccountID, Identifier, IsIn, IsOut) VALUES (@P1, @P2, @P3, @P4, @P5)";

            foreach (TABS.Switch definedSwitch in TABS.Switch.All.Values)
            {
                var manager = definedSwitch.SwitchManager;
                if (definedSwitch.SwitchManager != null)
                {
                    foreach (var carrier in TABS.CarrierAccount.All.Values)
                    {
                        foreach (string idIn in manager.GetCustomerCdrIdentifiers(definedSwitch, carrier))
                            TABS.DataHelper.ExecuteNonQuery(sql, definedSwitch.SwitchID, carrier.CarrierAccountID, idIn, 'Y', 'N');
                        foreach (string idOut in manager.GetSupplierCdrIdentifiers(definedSwitch, carrier))
                            TABS.DataHelper.ExecuteNonQuery(sql, definedSwitch.SwitchID, carrier.CarrierAccountID, idOut, 'N', 'Y');
                    }
                }
            }
        }

        public static DataTable GetSwitchCarrierMappings()
        {
            return TABS.DataHelper.GetDataTable("SELECT * FROM SwitchCarrierMapping ORDER BY CarrierAccountID, IsIn DESC, IsOut DESC");
        }

        public static void DropAndCreateSwitchCarrierMapping()
        {
            TABS.DataHelper.ExecuteNonQuery("DROP TABLE SwitchCarrierMapping; CREATE TABLE SwitchCarrierMapping(SwitchID TINYINT, CarrierAccountID VARCHAR(10), Identifier varchar(100), IsIn char(1), IsOut char(1))");
        }

        public static int UpdateRoutingStatus(CarrierAccount carrier, string enabled)
        {
            string sql;
            if (enabled == "Enabled")
            {
                sql = "Update CarrierAccount Set RoutingStatus = 0 where CarrierAccountID = @P1";
            }
            else
                sql = "Update CarrierAccount Set RoutingStatus = 3 where CarrierAccountID = @P1";
            return DataHelper.ExecuteNonQuery(sql, carrier.CarrierAccountID);
        }
    }
}
