
CREATE FUNCTION [COMMON].[getExchangeRates](@From DateTime, @To Datetime) 
RETURNS @ExchangeRates Table  
					(
						CurrencyID int NOT NULL,
						Rate Decimal(20,10) NOT NULL,
						BED DATETIME NOT NULL,
						EED DATETIME
					) 
AS
BEGIN
	
	INSERT INTO @ExchangeRates
	SELECT [CurrencyID]
		  ,[Rate]
		  ,[BED]
		  ,[EED]
	FROM [dbo].[CurrencyExchangeRate]
	WHERE EED IS NULL OR EED > @From
	
	
RETURN 
END