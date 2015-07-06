


CREATE PROCEDURE [dbo].[prGetToBeReported]
(
  @MobileOperatorID int = NULL,
  @ReceivedMobileOperatorID int = NULL,
  @SourceID int = NULL,
  @ReceivedSourceID int = NULL,
  @StatusID int = NULL,
  @ClientID int = NULL
)
AS
BEGIN  

                      
		Select 'Case' + CONVERT(char(20), GeneratedCalls.ID) AS CaseID, GeneratedCalls.MobileOperatorID, GeneratedCalls.StatusID, GeneratedCalls.ReportingStatusID, 
                      RecievedCalls.DurationInSeconds, GeneratedCalls.AttemptDateTime, Reports_1.ReportID AS ReportRealID, Sources_1.Name AS SourceName, 
                      GeneratedCalls.a_number, GeneratedCalls.b_number, Users.FullName AS MobileOperatorName, Sources.Name AS ReceivedSourceName, RecievedCalls.CLI, 
                      RecievedCalls.MobileOperatorID AS ReceivedMobileOperatorID, GeneratedCalls.ID, GeneratedCalls.SourceID, RecievedCalls.SourceID AS ReceivedSourceID, 
                      Statuses.Name AS StatusName, Reports.ReportID, RecievedCalls.ClientID
FROM         GeneratedCalls INNER JOIN
                      Statuses ON GeneratedCalls.StatusID = Statuses.ID LEFT OUTER JOIN
                      MobileOperators LEFT OUTER JOIN
                      Users ON MobileOperators.UserID = Users.ID ON GeneratedCalls.MobileOperatorID = MobileOperators.ID LEFT OUTER JOIN
                      RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID LEFT OUTER JOIN
                      Reports AS Reports_1 ON GeneratedCalls.ReportID = Reports_1.ID LEFT OUTER JOIN
                      Sources AS Sources_1 ON GeneratedCalls.SourceID = Sources_1.ID LEFT OUTER JOIN
                      Sources ON RecievedCalls.SourceID = Sources.ID LEFT OUTER JOIN
                      Reports RIGHT OUTER JOIN
                      RelatedNumbers ON Reports.ID = RelatedNumbers.ReportID ON RecievedCalls.CLI = RelatedNumbers.RelatedNumber
WHERE     (GeneratedCalls.ReportingStatusID = 4)
		
    
    and   (dbo.GeneratedCalls.StatusID=@StatusID or @StatusID=0)
    and   (dbo.GeneratedCalls.MobileOperatorID=@MobileOperatorID or @MobileOperatorID=0)
    and   (dbo.RecievedCalls.MobileOperatorID=@ReceivedMobileOperatorID or @ReceivedMobileOperatorID=0)
    and   (dbo.RecievedCalls.ClientID=@ClientID or @ClientID=0)
    and   (dbo.RecievedCalls.SourceID=@ReceivedSourceID or @ReceivedSourceID=0)
    and   (dbo.GeneratedCalls.SourceID=@SourceID or @SourceID=0)
     
	

                      
                      

END