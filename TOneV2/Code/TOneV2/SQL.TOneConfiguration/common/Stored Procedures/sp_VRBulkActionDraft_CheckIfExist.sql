-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE common.sp_VRBulkActionDraft_CheckIfExist
	@BulkActionDraftIdentifier uniqueidentifier
AS
BEGIN
	DECLARE @IdentifierExists bit
	SET @IdentifierExists = 0

	IF EXISTS(SELECT NULL FROM common.VRBulkActionDraft WHERE BulkActionDraftIdentifier = @BulkActionDraftIdentifier)
	BEGIN
		SET @IdentifierExists = 1
	END

	SELECT @IdentifierExists
	
END