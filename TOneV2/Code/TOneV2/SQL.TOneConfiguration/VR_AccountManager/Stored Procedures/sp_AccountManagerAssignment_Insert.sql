-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountManager].[sp_AccountManagerAssignment_Insert] 
	@AssignmentDefinitionID uniqueidentifier,
	@AccountManagerID bigint,
	@AccountID varchar(50),
	@Settings nvarchar(MAX),
	@BED datetime,
	@EED datetime,
    @ID INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM AccountManagerAssignment WHERE ID = @ID )
	BEGIN
		INSERT INTO VR_AccountManager.AccountManagerAssignment (AssignmentDefinitionID,AccountManagerID,AccountID,Settings, BED,EED)
		VALUES (@AssignmentDefinitionID,@AccountManagerID,@AccountID,@Settings, @BED,@EED)
		SET  @ID = scope_identity()
	END
END