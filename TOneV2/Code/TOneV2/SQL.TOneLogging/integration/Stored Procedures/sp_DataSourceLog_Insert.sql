-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE integration.sp_DataSourceLog_Insert 
	@DataSourceId int,
	@Severity int,
	@Message varchar(1000),
	@ImportedBatchId int,
	@LogEntryTime dateTime
AS
BEGIN
	Insert into integration.DataSourceLog (DataSourceId, Severity, [Message], ImportedBatchId, LogEntryTime)
    values (@DataSourceId, @Severity, @Message, @ImportedBatchId, @LogEntryTime)
END