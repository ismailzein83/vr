-- =========================================================
-- Author:		Fadi Chamieh
-- Create date: 20/09/2007
-- Description:	Get the Effective Zone Rates for a Customer
-- =========================================================
CREATE PROCEDURE [dbo].[bp_GetCustomerRates](@CustomerID varchar(10))

AS
BEGIN
	SET NOCOUNT ON;

	-- Select Custom Rates
    SELECT R.* FROM Rate R, PriceList P 
		WHERE 
				P.CustomerID = @CustomerID
			AND	R.IsEffective = 'Y'
			AND P.PriceListID=R.PriceListID
	ORDER BY ZoneID	
END