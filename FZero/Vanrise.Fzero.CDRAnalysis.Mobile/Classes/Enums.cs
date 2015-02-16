
namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public class Enums
    {
        public enum SystemPermissions : int
        {
            Dashboard= 1,
            Switches= 2,
            Trunks= 3,
            NormalizationRules= 4,
            UnNormalizationRules= 5,
            Strategies= 6,
            SuspectionAnalysis= 7,
            ReportManagement = 8,
            Users = 9,
            ManageUsers=10,
            ActivateDeactivateUser=11,
            ResetUserPassword=12,
            ManageSwitches=13,	
            ManageTrunks=14,	
            ManageNormalizationRules=15, 	
            AddRule=16 ,
            ManageStrategies=17	,
            AddCasestoReports=18,	
            ViewCaseDetails=19	,
            ManageReport=20	,
            SendReport=21	,
            EmailTemplates =22 ,
            EmailRecievers =23,
            ManageEmailTemplates = 24,
            ManageEmailRecievers = 25,
            SentEmails= 26,
            DeleteSentEmails =27, 
            ManualImport=28,
            SourcesMapping = 29
        }

        public enum ReportingStatuses : int
        {
            ToBeSent = 1,
            Sent = 2
        }

        public enum EmailRecieverTypes : int
        {
            To = 1,
            CC = 2,
            BCC =3

        }

        public enum EmailTemplates : int
        {
            ForgetPassword = 1,
            ReporttoITPC = 2
        }

        public enum ImportTypes : int
        {
            CDRs = 1
        }


        public enum SourceKinds : int
        {
            Database = 1, 
            File = 2
        }
        
    }
}