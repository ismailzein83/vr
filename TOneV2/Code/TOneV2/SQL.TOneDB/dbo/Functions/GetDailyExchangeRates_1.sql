-- ========================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-03-02
-- Description:	Get a table of Exchange Rates based on Date
-- =========================================================
CREATE FUNCTION [dbo].[GetDailyExchangeRates]
(
	-- Add the parameters for the function here
	@From DATETIME,
	@Till DATETIME
)
RETURNS 
@ExchangeRates TABLE 
(
	Currency VARCHAR(3),
	Date SMALLDATETIME,
	Rate FLOAT
	--PRIMARY KEY(Currency, Date)
)
AS
BEGIN
	SET @From = dbo.DateOf(@From)
	DECLARE @Pivot DATETIME
	SET @Pivot = @From
	WHILE @Pivot <= @Till
	BEGIN
		INSERT INTO @ExchangeRates(Currency, Date, Rate)
			SELECT c.CurrencyID, @Pivot, dbo.GetExchangeRate(c.CurrencyID, @Pivot) FROM Currency c WHERE c.IsVisible = 'Y'
		SET @Pivot = DATEADD(dd, 1, @Pivot)
	END
	RETURN
END