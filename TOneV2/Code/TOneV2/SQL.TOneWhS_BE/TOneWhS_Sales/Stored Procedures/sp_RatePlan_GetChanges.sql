-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_RatePlan_GetChanges]
	@OwnerType INT,
	@OwnerID INT,
	@Status INT
AS
BEGIN
	SELECT [Changes]
	FROM TOneWhS_Sales.RatePlan WITH(NOLOCK) 
	WHERE OwnerType = @OwnerType AND OwnerID = @OwnerID AND [Status] = @Status
END