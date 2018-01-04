-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SalePriceList_SetStateBackupIdForOwnerPricelists]
	@ProcessInstanceId BIGINT,
	@OwnerType INT,
	@OwnerID INT,
	@StateBackupId BIGINT
AS
BEGIN
	UPDATE TOneWhS_BE.SalePriceList_New
	SET [PricelistStateBackupID] = @StateBackupId
	WHERE OwnerType = @OwnerType AND OwnerID = @OwnerID AND [ProcessInstanceID] = @ProcessInstanceId
END