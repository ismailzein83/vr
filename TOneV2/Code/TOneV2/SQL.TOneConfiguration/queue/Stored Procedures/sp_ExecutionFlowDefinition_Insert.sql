
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_ExecutionFlowDefinition_Insert]
	@Id uniqueidentifier ,
	@Name Nvarchar(255),
    @Title nvarchar(225),
    @Stages nvarchar(max)

AS
BEGIN
	IF NOT EXISTS(select 1 from [queue].[ExecutionFlowDefinition] where Name = @Name)
	BEGIN
		Insert into [queue].[ExecutionFlowDefinition] (Id,[Name], [Title],[Stages])
		values(@Id,@Name, @Title,@Stages)
	END
END