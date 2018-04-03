-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRBulkActionDraft_Clear]
	@BulkActionDraftIdentifier uniqueidentifier,
	@RemoveBeforeDate datetime
AS
BEGIN
	DELETE common.VRBulkActionDraft 
	WHERE BulkActionDraftIdentifier = @BulkActionDraftIdentifier OR CreatedTime < @RemoveBeforeDate
END