-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_RatePlan_InsertOrUpdateDraftTaskData]
	@OwnerType INT,
	@OwnerID INT,
	@DraftTaskData NVARCHAR(MAX),
	@Status INT
AS
BEGIN
	UPDATE TOneWhS_Sales.RatePlan
	SET [DraftTaskData] = @DraftTaskData
	WHERE OwnerType = @OwnerType AND OwnerID = @OwnerID AND [Status] = @Status
	
	IF @@ROWCOUNT = 0 BEGIN
		INSERT INTO TOneWhS_Sales.RatePlan (OwnerType, OwnerID, [DraftTaskData], [Status])
		VALUES (@OwnerType, @OwnerID, @DraftTaskData, @Status)
	END
END