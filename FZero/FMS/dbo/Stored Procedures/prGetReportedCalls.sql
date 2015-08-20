


CREATE PROCEDURE [dbo].[prGetReportedCalls]
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
SELECT      'Case' + CONVERT(char(20), gc.ID) AS CaseID, gc.ReportingStatusID, gc.MobileOperatorFeedbackID, gc.AttemptDateTime, gc.FeedbackDateTime, gc.b_number, gc.ID, 
            rc.DurationInSeconds, rc.CLI, rs.Name AS ReportingStatusName, 
			r.ReportID AS ReportRealID, ra.Name AS RecommendedActionName, mof.Name AS MobileOperatorFeedbackName
FROM        dbo.GeneratedCalls gc with(nolock)
			INNER JOIN	dbo.RecievedCalls rc with(nolock) ON gc.ID = rc.GeneratedCallID 
			INNER JOIN	dbo.Reports r ON r.ID = gc.ReportID
			INNER JOIN	dbo.ReportingStatuses rs ON rs.ID = gc.ReportingStatusID
			LEFT JOIN	dbo.MobileOperatorFeedbacks mof ON gc.MobileOperatorFeedbackID = mof.ID			 
			LEFT JOIN	dbo.RecommendedActions ra	ON r.RecommendedActionID = ra.ID 

WHERE   gc.ReportingStatusID = 2
		and		(gc.MobileOperatorID=@B_NumberMobileOperatorID or @B_NumberMobileOperatorID=0)
		and		(gc.MobileOperatorFeedbackID=@MobileOperatorFeedbackID or @MobileOperatorFeedbackID=0)
		and		(@CaseID='' or   ('Case' + CONVERT(char(20), gc.ID)  like '%'+@CaseID+'%' ))
		and		(rc.ClientID=@ClientID or @ClientID=0)
		and		(r.ReportID      like '%' +@ReportID+'%' )
		and		(rc.MobileOperatorID=@CLIMobileOperatorID or @CLIMobileOperatorID=0)		
		and   (r.RecommendedActionID=@RecommendedActionID or @RecommendedActionID=0)		
		and   (@b_number='' or   (gc.b_number      like '%' +@b_number+'%' ))
		and   (@FromSentDateTime IS NULL OR r.SentDateTime >= @FromSentDateTime)
		and   (@ToSentDateTime IS NULL OR r.SentDateTime <= @ToSentDateTime)

END