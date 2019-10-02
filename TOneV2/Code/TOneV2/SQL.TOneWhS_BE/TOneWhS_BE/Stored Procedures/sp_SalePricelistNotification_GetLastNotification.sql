-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistNotification_GetLastNotification]
@CustomerIds nvarchar(max)
AS
BEGIN

	BEGIN
		DECLARE @CustomerIdsTable TABLE (CustomerID int)
		INSERT INTO @CustomerIdsTable (CustomerID)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds)

		--;with filteredNotification as (
		--Select pricelistId,customerId,EmailCreationDate,FileId
		--from [TOneWhS_BE].[SalePricelistNotification] with(nolock)
		--where 1=1
		--and (@CustomerIds is null or customerid in (select customerid from @CustomerIdsTable))
		--)
		--SELECT C.* 
		--FROM (SELECT DISTINCT customerId from filteredNotification) A
		--CROSS APPLY (SELECT TOP 1 * 
		--			 FROM filteredNotification B
		--			 WHERE A.customerId  = B.customerId
		--			 ORDER by EmailCreationDate desc) C

	;with filteredNotification as (	Select max(pricelistid) maxPricelistId,customerId
		from [TOneWhS_BE].[SalePricelistNotification] with(nolock)
		where 1=1
		and (@CustomerIds is null or customerid in (select customerid from @CustomerIdsTable))
		group by customerId
		)
		select spn.* 
		from [TOneWhS_BE].[SalePricelistNotification] spn with(nolock)
		join filteredNotification fn on fn.customerId = spn.customerID
	END
END