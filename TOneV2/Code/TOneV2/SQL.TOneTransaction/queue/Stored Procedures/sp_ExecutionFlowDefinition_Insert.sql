
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_ExecutionFlowDefinition_Insert]
	@Name Nvarchar(255),
    @Title nvarchar(225),
    @Stages nvarchar(max),
    @Id int out
AS
BEGIN
	IF NOT EXISTS(select 1 from [queue].[ExecutionFlowDefinition] where Name = @Name)
	BEGIN
		Insert into [queue].[ExecutionFlowDefinition] ([Name], [Title],[Stages])
		values(@Name, @Title,@Stages)
		
		SET @Id = SCOPE_IDENTITY()
	END
END