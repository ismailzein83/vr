




CREATE PROCEDURE [dbo].[prResultingCases]
(
           @CaseID varchar(100)=NULL,
           @SourceID int=NULL,
           @ReceivedSourceID int=NULL,
           @MobileOperatorID int=NULL,
           @StatusID int=NULL,
           @PriorityID int=NULL,
           @ReportingStatusID int=NULL,
           @a_number varchar(100)=NULL,
           @b_number varchar(100)=NULL,
           @CLI varchar(100)=NULL,
           @OriginationNetwork varchar(100)=NULL,
           @FromAttemptDateTime DateTime=NULL,
           @ToAttemptDateTime DateTime=NULL,
           @ClientID int=NULL
)
AS
BEGIN    


	
SELECT     dbo.GeneratedCalls.SourceID, 'Case' + CONVERT(char(20), dbo.GeneratedCalls.ID) AS CaseID, dbo.GeneratedCalls.MobileOperatorID, dbo.GeneratedCalls.StatusID, 
                      dbo.GeneratedCalls.PriorityID, dbo.GeneratedCalls.ReportingStatusID, dbo.RecievedCalls.DurationInSeconds, dbo.GeneratedCalls.MobileOperatorFeedbackID, 
                      dbo.GeneratedCalls.OriginationNetwork, dbo.GeneratedCalls.AssignedTo, dbo.GeneratedCalls.AssignedBy, dbo.GeneratedCalls.ReportID, 
                      dbo.GeneratedCalls.AttemptDateTime, dbo.GeneratedCalls.FeedbackDateTime, dbo.GeneratedCalls.AssignmentDateTime, 
                      dbo.ReportingStatuses.Name AS ReportingStatusName, dbo.Priorities.Name AS PriorityName, dbo.Reports.ReportID AS ReportRealID, 
                      Sources_1.Name AS SourceName, dbo.Statuses.Name AS StatusName, dbo.GeneratedCalls.a_number, dbo.GeneratedCalls.b_number, 
                      dbo.Reports.SentDateTime AS ReportingDateTime, dbo.Reports.RecommendedActionID, dbo.GeneratedCalls.Level1Comparison, 
                      dbo.GeneratedCalls.Level2Comparison, dbo.Sources.Name AS ReceivedSourceName, dbo.RecievedCalls.SourceID AS ReceivedSourceID, dbo.RecievedCalls.CLI, 
                      dbo.GeneratedCalls.ToneFeedbackID, dbo.GeneratedCalls.LevelOneComparisonDateTime, dbo.GeneratedCalls.LevelTwoComparisonDateTime, 
                      dbo.RecievedCalls.MobileOperatorID AS ReceivedMobileOperatorID, dbo.GeneratedCalls.ID, 'false' AS CLIReported, dbo.GeneratedCalls.FeedbackNotes, 
                      dbo.MobileOperatorFeedbacks.Name AS MobileOperatorFeedbackName, dbo.GeneratedCalls.Carrier, dbo.Clients.Name AS ClientName
FROM         dbo.Sources AS Sources_1 RIGHT OUTER JOIN
                      dbo.Reports RIGHT OUTER JOIN
                      dbo.GeneratedCalls WITH (NOLOCK) LEFT OUTER JOIN
                      dbo.MobileOperatorFeedbacks ON dbo.GeneratedCalls.MobileOperatorFeedbackID = dbo.MobileOperatorFeedbacks.ID LEFT OUTER JOIN
                      dbo.Clients INNER JOIN
                      dbo.RecievedCalls ON dbo.Clients.ID = dbo.RecievedCalls.ClientID ON dbo.GeneratedCalls.ID = dbo.RecievedCalls.GeneratedCallID ON 
                      dbo.Reports.ID = dbo.GeneratedCalls.ReportID LEFT OUTER JOIN
                      dbo.Statuses ON dbo.GeneratedCalls.StatusID = dbo.Statuses.ID ON Sources_1.ID = dbo.GeneratedCalls.SourceID LEFT OUTER JOIN
                      dbo.ReportingStatuses ON dbo.GeneratedCalls.ReportingStatusID = dbo.ReportingStatuses.ID LEFT OUTER JOIN
                      dbo.Priorities ON dbo.GeneratedCalls.PriorityID = dbo.Priorities.ID LEFT OUTER JOIN
                      dbo.Sources ON dbo.RecievedCalls.SourceID = dbo.Sources.ID
               WHERE            ( @FromAttemptDateTime is null or  dbo.GeneratedCalls.AttemptDateTime>=@FromAttemptDateTime) and (@ToAttemptDateTime is null or dbo.GeneratedCalls.AttemptDateTime<=@ToAttemptDateTime)
                            and (dbo.GeneratedCalls.StatusID=@StatusID or @StatusID=0)
                            and (dbo.GeneratedCalls.PriorityID=@PriorityID or @PriorityID=0)
                            and (dbo.GeneratedCalls.SourceID=@SourceID or @SourceID=0)
                            and (dbo.RecievedCalls.SourceID=@ReceivedSourceID or @ReceivedSourceID=0)
                            and (dbo.RecievedCalls.ClientID=@ClientID or @ClientID=0)
                            and (dbo.GeneratedCalls.OriginationNetwork like '%' +@OriginationNetwork + '%' or @OriginationNetwork='')
						    and (dbo.GeneratedCalls.a_number like '%' +@a_number + '%' or @a_number='')
						    and (dbo.GeneratedCalls.b_number like '%' +@b_number + '%' or @b_number='')
						    and (dbo.RecievedCalls.CLI like '%' +@CLI + '%' or @CLI='')
						    and (dbo.RecievedCalls.MobileOperatorID=@MobileOperatorID or @MobileOperatorID=0)
						    and (dbo.GeneratedCalls.ReportingStatusID=@ReportingStatusID or @ReportingStatusID=0)
                            and ('Case' + CONVERT(char(20), dbo.GeneratedCalls.ID) like '%' +@CaseID + '%' or @CaseID='')
                  
                  


END