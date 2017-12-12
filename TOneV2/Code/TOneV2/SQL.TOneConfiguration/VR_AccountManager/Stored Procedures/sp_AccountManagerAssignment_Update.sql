-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE  [VR_AccountManager].[sp_AccountManagerAssignment_Update] 
	@AccountManagerAssignementId bigint,
	@Settings nvarchar(MAX),
	@BED datetime,
	@EED datetime
AS
BEGIN

	Update VR_AccountManager.AccountManagerAssignment
	Set  
		Settings=@Settings,
		BED = @BED,
		EED=@EED
	Where ID = @AccountManagerAssignementId
	END