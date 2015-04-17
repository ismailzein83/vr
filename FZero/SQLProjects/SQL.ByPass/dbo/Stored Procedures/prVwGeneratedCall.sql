

















CREATE PROCEDURE [dbo].[prVwGeneratedCall]
(
 @ID INT = NULL
)
AS
BEGIN    


	SELECT     dbo.RecievedCalls.ClientID, dbo.GeneratedCalls.SourceID, 'Case' + CONVERT(char(20), dbo.GeneratedCalls.ID) AS CaseID, dbo.GeneratedCalls.MobileOperatorID, 
                      dbo.GeneratedCalls.StatusID, dbo.GeneratedCalls.PriorityID, dbo.GeneratedCalls.ReportingStatusID, dbo.GeneratedCalls.DurationInSeconds, 
                      dbo.GeneratedCalls.MobileOperatorFeedbackID, dbo.GeneratedCalls.OriginationNetwork, dbo.GeneratedCalls.AssignedTo, dbo.GeneratedCalls.AssignedBy, 
                      dbo.GeneratedCalls.ReportID, dbo.GeneratedCalls.AttemptDateTime, dbo.GeneratedCalls.FeedbackDateTime, dbo.GeneratedCalls.AssignmentDateTime, 
                      dbo.ReportingStatuses.Name AS ReportingStatusName, dbo.Priorities.Name AS PriorityName, dbo.Reports.ReportID AS ReportRealID, 
                      Sources_1.Name AS SourceName, dbo.Statuses.Name AS StatusName, Users_1.FullName AS AssignedToFullName, dbo.GeneratedCalls.a_number, 
                      dbo.GeneratedCalls.b_number, dbo.Users.FullName AS MobileOperatorName, dbo.RecommendedActions.Name AS RecommendedActionName, 
                      dbo.Reports.SentDateTime AS ReportingDateTime, Users_2.FullName AS ReportedByName, Users_2.MobileNumber, dbo.Reports.RecommendedActionID, 
                      dbo.GeneratedCalls.Level1Comparison, dbo.GeneratedCalls.Level2Comparison, dbo.Sources.Name AS ReceivedSourceName, 
                      dbo.RecievedCalls.SourceID AS ReceivedSourceID, dbo.RecievedCalls.CLI, dbo.GeneratedCalls.ToneFeedbackID, dbo.GeneratedCalls.LevelOneComparisonDateTime, 
                      dbo.GeneratedCalls.LevelTwoComparisonDateTime, dbo.ToneFeedbacks.Name AS ToneFeedbackName, 
                      dbo.RecievedCalls.MobileOperatorID AS ReceivedMobileOperatorID, Users_3.FullName AS ReceivedMobileOperatorFeedbackName, dbo.GeneratedCalls.ID, 
                      dbo.GeneratedCalls.FeedbackNotes, dbo.MobileOperatorFeedbacks.Name AS MobileOperatorFeedbackName, dbo.GeneratedCalls.Carrier
FROM         dbo.ReportingStatuses RIGHT OUTER JOIN
                      dbo.Statuses RIGHT OUTER JOIN
                      dbo.Reports LEFT OUTER JOIN
                      dbo.ApplicationUsers ON dbo.Reports.ApplicationUserID = dbo.ApplicationUsers.ID RIGHT OUTER JOIN
                      dbo.Users AS Users_1 RIGHT OUTER JOIN
                      dbo.ApplicationUsers AS ApplicationUsers_1 ON Users_1.ID = ApplicationUsers_1.UserID RIGHT OUTER JOIN
                      dbo.GeneratedCalls LEFT OUTER JOIN
                      dbo.MobileOperatorFeedbacks ON dbo.GeneratedCalls.MobileOperatorFeedbackID = dbo.MobileOperatorFeedbacks.ID ON 
                      ApplicationUsers_1.ID = dbo.GeneratedCalls.AssignedTo LEFT OUTER JOIN
                      dbo.MobileOperators LEFT OUTER JOIN
                      dbo.Users ON dbo.MobileOperators.UserID = dbo.Users.ID ON dbo.GeneratedCalls.MobileOperatorID = dbo.MobileOperators.ID LEFT OUTER JOIN
                      dbo.ToneFeedbacks ON dbo.GeneratedCalls.ToneFeedbackID = dbo.ToneFeedbacks.ID LEFT OUTER JOIN
                      dbo.Users AS Users_3 RIGHT OUTER JOIN
                      dbo.MobileOperators AS MobileOperators_1 RIGHT OUTER JOIN
                      dbo.RecievedCalls ON MobileOperators_1.ID = dbo.RecievedCalls.MobileOperatorID ON Users_3.ID = MobileOperators_1.UserID ON 
                      dbo.GeneratedCalls.ID = dbo.RecievedCalls.GeneratedCallID ON dbo.Reports.ID = dbo.GeneratedCalls.ReportID LEFT OUTER JOIN
                      dbo.RecommendedActions ON dbo.Reports.RecommendedActionID = dbo.RecommendedActions.ID ON 
                      dbo.Statuses.ID = dbo.GeneratedCalls.StatusID LEFT OUTER JOIN
                      dbo.Sources AS Sources_1 ON dbo.GeneratedCalls.SourceID = Sources_1.ID ON dbo.ReportingStatuses.ID = dbo.GeneratedCalls.ReportingStatusID LEFT OUTER JOIN
                      dbo.Priorities ON dbo.GeneratedCalls.PriorityID = dbo.Priorities.ID LEFT OUTER JOIN
                      dbo.Sources ON dbo.RecievedCalls.SourceID = dbo.Sources.ID LEFT OUTER JOIN
                      dbo.Users AS Users_2 ON dbo.ApplicationUsers.UserID = Users_2.ID
                      
                      where GeneratedCalls.ID=@ID







END