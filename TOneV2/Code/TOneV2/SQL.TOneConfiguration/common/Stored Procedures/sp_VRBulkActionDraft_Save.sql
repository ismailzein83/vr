-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE common.sp_VRBulkActionDraft_Save
	@VRBulkActionDraftTable common.VRBulkActionDraftTable readonly
AS
BEGIN

    INSERT INTO common.VRBulkActionDraft(BulkActionDraftIdentifier, ItemId)
	SELECT BulkActionDraftIdentifier, ItemId
	FROM @VRBulkActionDraftTable
END