-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_RatePlan_SetStatusIfExists]
	@OwnerType INT,
	@OwnerId INT,
	@Status INT
AS
BEGIN
	UPDATE TOneWhS_Sales.RatePlan
	SET [Status] = @Status
	WHERE OwnerType = @OwnerType AND OwnerID = @OwnerId
END