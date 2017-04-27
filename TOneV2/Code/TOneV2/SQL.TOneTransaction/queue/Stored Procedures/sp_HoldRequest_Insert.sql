-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_HoldRequest_Insert]
	@BPInstanceID bigint,
	@ExecutionFlowDefinitionId uniqueidentifier,
	@From datetime,
	@To datetime,
	@QueuesToHold  nvarchar(max),
	@QueuesToProcess nvarchar(max),
	@Status int
AS
BEGIN
	INSERT INTO [queue].[HoldRequest] ([BPInstanceID],[ExecutionFlowDefinitionId], [From], [To], [QueuesToHold], [QueuesToProcess],[Status])
     VALUES (@BPInstanceID,@ExecutionFlowDefinitionId, @From, @To, @QueuesToHold, @QueuesToProcess, @Status)
END