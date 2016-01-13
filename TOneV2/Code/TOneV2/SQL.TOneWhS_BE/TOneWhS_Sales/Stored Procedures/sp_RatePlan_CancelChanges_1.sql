-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_RatePlan_CancelChanges]
	@OwnerType TINYINT,
	@OwnerID INT
AS
BEGIN
	UPDATE TOneWhS_Sales.RatePlan
	SET [Status] = 2 -- Cancelled
	WHERE OwnerType = @OwnerType
		AND OwnerID = @OwnerID
		AND [Status] = 0 -- Draft
END