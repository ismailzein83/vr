-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceLog_Insert] 
	@DataSourceId uniqueidentifier,
	@Severity int,
	@Message nvarchar(max),
	@ImportedBatchId bigint,
	@LogEntryTime dateTime
AS
BEGIN
	Insert into integration.DataSourceLog (DataSourceId, Severity, [Message], ImportedBatchId, LogEntryTime)
    values (@DataSourceId, @Severity, @Message, @ImportedBatchId, @LogEntryTime)
END