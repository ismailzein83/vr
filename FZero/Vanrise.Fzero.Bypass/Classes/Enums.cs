
namespace Vanrise.Fzero.Bypass
{
    public class Enums
    {
        public enum SystemPermissions : int
        {
            	ViewSystemParameters=8,
	            EditSystemParameters=9,
	            ViewEmailTemplates=10,
	            EditEmailTemplate=11,
	            ViewLoggedActions=12,
	            DeleteLoggedActions=13,
	            ViewEmails=14,
	            DeleteEmails=15,
	            ViewMobileOperators=18,
	            EditMobileOperator=19,
	            ManageApplicationUsers=32,
	            ResetApplicationUserPassword=33,
	            ActivateDeactivateApplicationUser=34,
	            ViewResultedCalls=36,
	            ChangeReportingStatus=37,
	            ReporttoOperator=38,
                EditSources = 39,
                SourcesMapping = 40,
                ManualImport = 41,
                MonitorReportedCases=42,
                CaseAssignment = 43,
                Dashboard = 45,
                MonitoringnTracking = 44,
                RelatedNumberMappings = 46,
                RelatedNumbers = 47
        }

        public enum EmailTemplates : int
        {
            ForgetPassword = 1,
            ReporttoMobileOperator = 2,
            MobileOperatorFeedbackonReportedCases = 3,
            DailyReport = 4,
            ReporttoMobileSyrianOperator = 5,
            SyrianDailyReport = 6,
            WeeklyReport = 7,
            SyrianWeeklyReport = 8,
            RepeatedReporttoMobileOperator = 9,
            RepeatedReporttoMobileSyrianOperator = 10,
            AutoBlockReport = 11,
            ReporttoMobileOperatorNoBLock = 12,
            ReporttoMobileSyrianOperatorNoBlock = 13
        }

        public enum Clients : int
        {
            ITPC = 1,
            Zain = 2,
            ST = 3
        }

        public enum RecommendedAction : int
        {
            Block = 1,
            Investigate = 2,
        }


        public enum ActionTypes : int
        {
            ActivationDeactivationofanApplicationUser=	1,
            ResetApplicationUsersPassword=	2,
            UpdateaSystemParameter	=3,
            Repliedonreportedcases = 4,
            Reportcasestomobileoperator=5,
            Importedgeneratedcalls=7, 
            Updatedsourcemapping=8,
            ChangedcasesreportingstatusIgnored=9,
            ChangedcasesreportingstatusToBeReported=10,
            UpdatedRelatedNumberMapping=11,
            AddedRelatedNumbers = 12
            
        }

        public enum ReportingStatuses : int
        {
                Pending = 1,
	            Reported=2,
	            Ignored=3,
	            TobeReported=4,
                Verified=5,
                Reopened =6, 
                TobeInvestigated=7
        }

        public enum ChangeType : int
        {
           	ChangedStatus= 1,
	        ChangedReportingStatus= 2,
	        ChangedMobileOperatorFeedback= 3
        }

        public enum Statuses : int
        {
            	Pending=1,
	            Fraud=2,
	            Suspect=3,
	            Clean=4,
                Ignored = 5,
                DistintFraud = 6
        }



        public enum SourceTypes : int
        {
            GeneratesOnly=1,
            RecievesOnly=2,
            GeneratesandRecieves=3
        }

        public enum FilterTypes : int
        {
            Contains= 0,
            StartsWith= 1,
            EndsWith= 2,
        }


         public enum ImportTypes : int
         {
            GeneratedCalls=1,
            ReceivedCalls=2
         }


         public enum MobileOperatorFeedbacks : int
         {
             Pending = 1,
             Rejected = 2,
             Blocked = 3
         }

         public enum ToneFeedback : int
         {
             Found = 1,
             NotFound = 2,
             DatanotAvailable = 3
         }



       
    }
}