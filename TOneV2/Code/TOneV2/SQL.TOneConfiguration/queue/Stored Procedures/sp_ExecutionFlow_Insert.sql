-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_ExecutionFlow_Insert]
	@Id uniqueidentifier ,
	@Name Nvarchar(255),
	@DefinitionId uniqueidentifier

AS
BEGIN
	IF NOT EXISTS(select 1 from [queue].[ExecutionFlow] where Name = @Name)
	BEGIN
		Insert into [queue].[ExecutionFlow] (ID,[Name], [ExecutionFlowDefinitionID])
		values(@ID,@Name, @DefinitionId)

	END
END