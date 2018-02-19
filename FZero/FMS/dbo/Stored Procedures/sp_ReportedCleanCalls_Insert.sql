-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE dbo.sp_ReportedCleanCalls_Insert
@GeneratedCallsIds NVARCHAR(MAX)
AS
BEGIN
	    DECLARE @GeneratedCallsIdsTable TABLE (GeneratedCallId INT)
		INSERT  INTO  @GeneratedCallsIdsTable (GeneratedCallId)
		select ParsedString from dbo.[ParseStringList](@GeneratedCallsIds)
		
		INSERT INTO  dbo.ReportedCleanCalls SELECT GeneratedCallId FROM @GeneratedCallsIdsTable
END