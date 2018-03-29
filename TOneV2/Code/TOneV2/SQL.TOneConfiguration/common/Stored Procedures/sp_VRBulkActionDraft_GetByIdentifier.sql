-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE common.sp_VRBulkActionDraft_GetByIdentifier 
	@BulkActionDraftIdentifier uniqueidentifier
AS
BEGIN
	SELECT ID, ItemId, BulkActionDraftIdentifier 
	FROM common.VRBulkActionDraft 
	WHERE BulkActionDraftIdentifier = @BulkActionDraftIdentifier
END