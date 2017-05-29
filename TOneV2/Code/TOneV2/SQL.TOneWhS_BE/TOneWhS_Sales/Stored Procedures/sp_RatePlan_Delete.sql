-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_RatePlan_Delete
	@OwnerType tinyint,
	@OwnerID int
AS
BEGIN
	delete from TOneWhS_Sales.RatePlan where OwnerType = @OwnerType and OwnerID = @OwnerID
END