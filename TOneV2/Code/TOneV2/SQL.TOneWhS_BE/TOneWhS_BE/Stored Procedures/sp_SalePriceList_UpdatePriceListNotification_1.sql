-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_UpdatePriceListNotification]
	@CustomerID nvarchar(max),
	@OwnerType int,
	@processInstanceId int
AS
BEGIN
	DECLARE @CustomerIDsTable TABLE (CustomerID int)
	INSERT INTO @CustomerIDsTable (CustomerID)
	SELECT Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerID)
	Update TOneWhS_BE.SalePriceList
	Set IsSent = 1
	Where  (@CustomerID  is null or OwnerID in (select CustomerID from @CustomerIDsTable))
	AND ownertype = @OwnerType

END