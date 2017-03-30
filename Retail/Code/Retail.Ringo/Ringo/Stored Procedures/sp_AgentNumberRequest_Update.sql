CREATE PROCEDURE [Ringo].[sp_AgentNumberRequest_Update]
	@ID int,
	@Status tinyint,
	@Settings nvarchar(MAX)
AS
BEGIN


		Update [Ringo].[AgentNumberRequest]
	Set  Settings = @Settings,Status = @Status
	Where ID = @ID

END