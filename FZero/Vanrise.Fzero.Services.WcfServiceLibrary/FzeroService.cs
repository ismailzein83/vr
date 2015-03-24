using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.Bypass;


namespace Vanrise.Fzero.Services.WcfServiceLibrary
{
    public class FzeroService : IFzeroService
    {
        public void Import(string filePath, string SourceName) // Import Calls
         {
             DataTable dt = null;
              if (filePath.Contains(".xls"))
                 {
                      dt = GeneratedCall.GetDataFromExcel(filePath, SourceName);
                 }

              else if (filePath.Contains(".xlsx"))
                 {
                      dt = GeneratedCall.GetDataFromExcel(filePath, SourceName);
                 }

             
              else if (filePath.Contains(".xml"))
                  {
                      dt = GeneratedCall.GetDataFromXml(filePath, SourceName);
                  }
            if(dt != null)
            {
                GeneratedCall.Confirm(SourceName, dt, null);
            }
              
            
        }

        public string  GetSourceNameByEmail(string Email) // Get Folder of a Given Source Knowing its Email Address;
        {
            return Source.GetByEmail(Email).Name;
        }

        public int GetSourceIDByEmail(string Email) // Get Folder of a Given Source Knowing its Email Address;
        {
            return Source.GetByEmail(Email).ID;
        }

        public RecievedEmail GetLastEmailRecieved(int Source) // Get Last Recieved Email;
        {
            return RecievedEmail.GetLast(Source);
        }

        public RecievedEmail SaveLastEmailRecieved(RecievedEmail RecievedEmail)
        {
            return RecievedEmail.Save(RecievedEmail);
        }

        public List<generatedCall> GetCallsDidNotPassLevelTwo(bool LevelTwoComparisonIsObligatory)
        {
            List<generatedCall> listvGC = new List<generatedCall>();
            List<GeneratedCall> listVGC = GeneratedCall.GetCallsDidNotPassLevelTwo(LevelTwoComparisonIsObligatory);

            foreach (GeneratedCall VGC in listVGC)
            {
                generatedCall vGC = new generatedCall();

                 vGC.ID = VGC.ID;
                 vGC.SourceID = VGC.SourceID;
                 vGC.MobileOperatorID = VGC.MobileOperatorID;
                 vGC.StatusID = VGC.StatusID;
                 vGC.PriorityID = VGC.PriorityID;
                 vGC.ReportingStatusID = VGC.ReportingStatusID;
                 vGC.DurationInSeconds = VGC.DurationInSeconds;
                 vGC.MobileOperatorFeedbackID = VGC.MobileOperatorFeedbackID;
                 vGC.a_number = VGC.a_number;
                 vGC.b_number = VGC.b_number;
                 vGC.CLI = VGC.CLI;
                 vGC.OriginationNetwork = VGC.OriginationNetwork;
                 vGC.AssignedTo = VGC.AssignedTo;
                 vGC.AssignedBy = VGC.AssignedBy;
                 vGC.ReportID = VGC.ReportID;
                 vGC.AttemptDateTime = VGC.AttemptDateTime;
                 vGC.LevelOneComparisonDateTime = VGC.LevelOneComparisonDateTime;
                 vGC.LevelTwoComparisonDateTime = VGC.LevelTwoComparisonDateTime;
                 vGC.FeedbackDateTime = VGC.FeedbackDateTime;
                 vGC.AssignmentDateTime = VGC.AssignmentDateTime;
                 vGC.ImportID = VGC.ImportID;
                 vGC.ReportingStatusChangedBy = VGC.ReportingStatusChangedBy;
                 vGC.Level1Comparison = VGC.Level1Comparison;
                 vGC.Level2Comparison = VGC.Level2Comparison;
                 vGC.ToneFeedbackID = VGC.ToneFeedbackID;
                 vGC.FeedbackNotes = VGC.FeedbackNotes;

                listvGC.Add(vGC);
            }
            return listvGC;
        }

        public void PerformLevelTwoComparison(List<generatedCall> GeneratedCallsList)
        {

            List<GeneratedCall> listVGC = new List<GeneratedCall>();

            foreach (generatedCall VGC in GeneratedCallsList)
            {
                GeneratedCall vGC = new GeneratedCall();
                vGC.SourceID = VGC.SourceID;
                vGC.MobileOperatorID = VGC.MobileOperatorID;
                vGC.StatusID = VGC.StatusID;
                vGC.PriorityID = VGC.PriorityID;
                vGC.ReportingStatusID = VGC.ReportingStatusID;
                vGC.DurationInSeconds = VGC.DurationInSeconds;
                vGC.MobileOperatorFeedbackID = VGC.MobileOperatorFeedbackID;
                vGC.a_number = VGC.a_number;
                vGC.b_number = VGC.b_number;
                vGC.CLI = VGC.CLI;
                vGC.OriginationNetwork = VGC.OriginationNetwork;
                vGC.AssignedTo = VGC.AssignedTo;
                vGC.AssignedBy = VGC.AssignedBy;
                vGC.ReportID = VGC.ReportID;
                vGC.AttemptDateTime = VGC.AttemptDateTime;
                vGC.LevelOneComparisonDateTime = VGC.LevelOneComparisonDateTime;
                vGC.LevelTwoComparisonDateTime = VGC.LevelTwoComparisonDateTime;
                vGC.FeedbackDateTime = VGC.FeedbackDateTime;
                vGC.AssignmentDateTime = VGC.AssignmentDateTime;
                vGC.ImportID = VGC.ImportID;
                vGC.ReportingStatusChangedBy = VGC.ReportingStatusChangedBy;
                vGC.Level1Comparison = VGC.Level1Comparison;
                vGC.Level2Comparison = VGC.Level2Comparison;
                vGC.ToneFeedbackID = VGC.ToneFeedbackID;
                vGC.FeedbackNotes = VGC.FeedbackNotes;



                listVGC.Add(vGC);
            }

            GeneratedCall.PerformLevelTwoComparison(listVGC);
        }


    }
}
