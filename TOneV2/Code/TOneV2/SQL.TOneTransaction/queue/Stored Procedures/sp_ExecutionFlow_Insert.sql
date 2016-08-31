-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_ExecutionFlow_Insert]
	@Name Nvarchar(255),
	@DefinitionId int,
	@Id int out
AS
BEGIN
	IF NOT EXISTS(select 1 from [queue].[ExecutionFlow] where Name = @Name)
	BEGIN
		Insert into [queue].[ExecutionFlow] ([Name], [ExecutionFlowDefinitionID])
		values(@Name, @DefinitionId)
		
		SET @Id = SCOPE_IDENTITY()
	END
END