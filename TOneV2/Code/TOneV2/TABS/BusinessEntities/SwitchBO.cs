
namespace TABS.BusinessEntities
{
    public class SwitchBO
    {
        public static void DeleteSwitchReleaseCodes(int switchID)
        {
            TABS.DataHelper.ExecuteNonQuery("DELETE FROM SwitchReleaseCode WHERE SwitchID=@P1", switchID);
        }

        public static void AddSwitchHistory(Switch EditedSwitch)
        {
            TABS.DataHelper.ExecuteNonQuery
                    (@"Exec bp_AddSwitchHistory
                        @SwitchID = @P1,
                        @Symbol = @P2,
                        @Name = @P3,
                        @Description = @P4,
                        @Configuration = @P5,
                        @LastCDRImportTag = @P6,
                        @LastImport = @P7,
                        @LastRouteUpdate = @P8,
                        @UserID = @P9,
                        @Enable_CDR_Import = @P10,
                        @Enable_Routing = @P11,
                        @LastAttempt = @P12",
                        EditedSwitch.SwitchID, EditedSwitch.Symbol, EditedSwitch.Name, EditedSwitch.Description,
                        EditedSwitch.ConfigurationXmlString, EditedSwitch.LastCDRImportTag, EditedSwitch.LastImport,
                        EditedSwitch.LastRouteUpdate, EditedSwitch.User.ID, EditedSwitch.Enable_CDR_Import ? "Y" : "N",
                        EditedSwitch.Enable_Routing ? "Y" : "N", EditedSwitch.LastAttempt);
        }
    }
}
