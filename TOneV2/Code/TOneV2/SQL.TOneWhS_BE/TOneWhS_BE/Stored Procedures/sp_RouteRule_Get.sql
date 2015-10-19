-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhS_BE].sp_RouteRule_Get
	@RouteRuleId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [ID]
		  ,[Criteria]
		  ,[TypeConfigID]
		  ,[RuleSettings]
		  ,[Description]
		  ,[BED]
		  ,[EED]
		  ,[ScheduleSettings]
      from TOneWhS_BE.RouteRule
      where ID = @RouteRuleId 
END