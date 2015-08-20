



CREATE PROCEDURE [dbo].[prGetReportedCalls_old]
(
  @CaseID varchar(30) = NULL,
  @b_number varchar(30) = NULL,
  @CLI varchar(30) = NULL,
  @FromSentDateTime DateTime = NULL,
  @ToSentDateTime DateTime = NULL,
  @ReportID varchar(30) = NULL,
  @CLIMobileOperatorID int = NULL,
  @B_NumberMobileOperatorID int = NULL,
  @MobileOperatorFeedbackID int = NULL,
  @RecommendedActionID int = NULL,
  @ClientID int = NULL
)
AS
BEGIN  

                      
SELECT        'Case' + CONVERT(char(20), dbo.GeneratedCalls.ID) AS CaseID, dbo.GeneratedCalls.ReportingStatusID, dbo.GeneratedCalls.MobileOperatorFeedbackID, 
                         dbo.GeneratedCalls.AttemptDateTime, dbo.GeneratedCalls.FeedbackDateTime, dbo.GeneratedCalls.b_number, dbo.GeneratedCalls.ID, 
                         dbo.RecievedCalls.DurationInSeconds, dbo.RecievedCalls.CLI, dbo.ReportingStatuses.Name AS ReportingStatusName, dbo.Reports.ReportID AS ReportRealID, 
                         dbo.RecommendedActions.Name AS RecommendedActionName, dbo.MobileOperatorFeedbacks.Name AS MobileOperatorFeedbackName
FROM            dbo.ReportingStatuses RIGHT OUTER JOIN
                         dbo.Reports RIGHT OUTER JOIN
                         dbo.GeneratedCalls LEFT OUTER JOIN
                         dbo.MobileOperatorFeedbacks ON dbo.GeneratedCalls.MobileOperatorFeedbackID = dbo.MobileOperatorFeedbacks.ID LEFT OUTER JOIN
                         dbo.RecievedCalls ON dbo.GeneratedCalls.ID = dbo.RecievedCalls.GeneratedCallID ON dbo.Reports.ID = dbo.GeneratedCalls.ReportID LEFT OUTER JOIN
                         dbo.RecommendedActions ON dbo.Reports.RecommendedActionID = dbo.RecommendedActions.ID ON 
                         dbo.ReportingStatuses.ID = dbo.GeneratedCalls.ReportingStatusID


WHERE     (GeneratedCalls.ReportingStatusID = 2)
    and (@CaseID='' or   ('Case' + CONVERT(char(20), dbo.GeneratedCalls.ID)  like '%'+@CaseID+'%'   ))
    and   (dbo.Reports.ReportID      like '%' +@ReportID+'%' )
    and   (dbo.RecievedCalls.MobileOperatorID=@CLIMobileOperatorID or @CLIMobileOperatorID=0)
    and   (dbo.GeneratedCalls.MobileOperatorID=@B_NumberMobileOperatorID or @B_NumberMobileOperatorID=0)
    and   (dbo.GeneratedCalls.MobileOperatorFeedbackID=@MobileOperatorFeedbackID or @MobileOperatorFeedbackID=0)
    and   (dbo.Reports.RecommendedActionID=@RecommendedActionID or @RecommendedActionID=0)
    and   (dbo.RecievedCalls.ClientID=@ClientID or @ClientID=0)
	and   (@b_number='' or   (dbo.GeneratedCalls.b_number      like '%' +@b_number+'%' ))
	and   (@FromSentDateTime IS NULL OR dbo.Reports.SentDateTime >= @FromSentDateTime)
    and   (@ToSentDateTime IS NULL OR dbo.Reports.SentDateTime <= @ToSentDateTime)

END