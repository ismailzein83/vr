-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_RatePlan_GetDraftTaskData]
	@OwnerType INT,
	@OwnerID INT,
	@Status INT
AS
BEGIN
	SELECT [DraftTaskData]
	FROM TOneWhS_Sales.RatePlan WITH(NOLOCK) 
	WHERE OwnerType = @OwnerType AND OwnerID = @OwnerID AND [Status] = @Status
END