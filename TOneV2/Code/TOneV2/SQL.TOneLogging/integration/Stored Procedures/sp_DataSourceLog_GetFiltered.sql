-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceLog_GetFiltered]
	@DataSourceId uniqueidentifier = NULL,
	@Severities nvarchar(max),
	@From DATETIME = NULL,
	@To DATETIME = NULL,
	@Top int
AS

SET NOCOUNT ON;

	BEGIN
		DECLARE @SeveritiesTable TABLE (Severity int)
		INSERT INTO @SeveritiesTable (Severity)
		select Convert(int, ParsedString) from [bp].[ParseStringList](@Severities)

	select TOP(@Top) [ID],
			[DataSourceId],
			[Severity],
			[Message],
			[LogEntryTime]
	FROM [integration].[DataSourceLog] as ib WITH(NOLOCK) 
	WHERE 
		(@DataSourceId IS NULL OR DataSourceId = @DataSourceId)
		AND (@Severities is null or Severity IN (SELECT Severity FROM @SeveritiesTable)) 
		AND (@From IS NULL OR LogEntryTime >= @From) 
		AND (@To IS NULL OR LogEntryTime <= @To)
		ORDER BY ID DESC
	END
SET NOCOUNT OFF