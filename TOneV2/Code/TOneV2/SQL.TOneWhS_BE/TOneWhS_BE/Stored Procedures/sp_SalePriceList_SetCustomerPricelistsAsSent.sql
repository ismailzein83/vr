-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_SetCustomerPricelistsAsSent]
	@CustomerID nvarchar(max),
	@PriceListId int = null
AS
BEGIN
	DECLARE @CustomerIDsTable TABLE (CustomerID int)
	INSERT INTO @CustomerIDsTable (CustomerID)
	SELECT Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerID)
	
	Update TOneWhS_BE.SalePriceList Set IsSent = 1
	Where Ownertype = 1 AND OwnerID in (select CustomerID from @CustomerIDsTable)
	and (ID=@PriceListId)
END