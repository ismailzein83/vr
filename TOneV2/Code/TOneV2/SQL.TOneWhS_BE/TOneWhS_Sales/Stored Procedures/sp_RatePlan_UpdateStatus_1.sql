-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_RatePlan_UpdateStatus
	@OwnerType INT,
	@OwnerID INT,
	@ExistingStatus INT,
	@NewStatus INT
AS
BEGIN
	UPDATE TOneWhS_Sales.RatePlan
	SET [Status] = @NewStatus
	WHERE OwnerType = @OwnerType AND OwnerID = @OwnerID AND [Status] = @ExistingStatus
END