-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Ringo.sp_AgentNumberRequest_Insert 
	-- Add the parameters for the stored procedure here
	@AgentId bigint,
	@Settings nvarchar(max),
	@Status tinyint,
	@id INT OUT
AS
BEGIN
INSERT INTO [Ringo].[AgentNumberRequest]
           ([AgentId],[Settings],[Status])
     VALUES
           (@AgentId,@Settings,@Status)
	SET @id = SCOPE_IDENTITY()
end