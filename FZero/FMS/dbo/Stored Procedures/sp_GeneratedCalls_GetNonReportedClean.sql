-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE dbo.[sp_GeneratedCalls_GetNonReportedClean]
@MobileOperatorId int,
@ClientID int 
AS
BEGIN
	  SELECT gc.[ID]
      ,rc.[DurationInSeconds]
      ,[MobileOperatorFeedbackID]
      ,gc.[a_number]
      ,rc.[b_number]
      ,gc.[CLI] as GeneratedCLI
	  ,rc.[CLI]as RecievedCLI
      ,gc.[AttemptDateTime]
	  ,rcc.GeneratedCallID
      ,Case When gc.[CLI] = rc.[CLI] THEN 0 ELSE 1 END as DifferentCLI
      FROM [dbo].[GeneratedCalls] gc
	  Join [dbo].RecievedCalls rc on rc.GeneratedCallID = gc.ID
	  left join dbo.ReportedCleanCalls rcc on rcc.GeneratedCallID = gc.ID
	  WHERE rcc.GeneratedCallID IS NULL
	  AND rc.[MobileOperatorID] = @MobileOperatorId AND (@ClientID IS NULL OR rc.ClientID = @ClientID)
END