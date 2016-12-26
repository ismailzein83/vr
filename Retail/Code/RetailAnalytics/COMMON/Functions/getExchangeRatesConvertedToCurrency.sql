CREATE FUNCTION [COMMON].[getExchangeRatesConvertedToCurrency](@CurrencyIDToConvertTo int, @From DateTime, @To Datetime) 
RETURNS @ConvertedExchangeRates Table  
					(
						CurrencyID int NOT NULL,
						Rate Decimal(20,10) NOT NULL,
						BED DATETIME NOT NULL,
						EED DATETIME
					) 
AS
BEGIN
	WITH originalExchangeRates AS 
							(
								SELECT * FROM Common.getExchangeRates(@From, @To)
							)
	INSERT INTO @ConvertedExchangeRates
	SELECT exRate1.CurrencyID, exRate1.Rate/ exRate2.Rate as Rate ,
		CASE WHEN exRate1.BED >= exRate2.BED THEN exRate1.BED ELSE exRate2.BED END BED
	 , CASE WHEN ISNULL(exRate1.EED, '9999-1-1') > ISNULL(exRate2.EED, '9999-1-1') THEN exRate2.EED ELSE exRate1.EED END EED
	FROM originalExchangeRates as exRate1 join originalExchangeRates as exRate2 on exRate2.CurrencyID = @CurrencyIDToConvertTo 
												and ((exRate1.BED >= exRate2.BED AND (exRate2.EED IS NULL OR exRate2.EED > exRate1.BED))
														OR
													 (exRate1.BED < exRate2.BED AND (exRate1.EED IS NULL OR exRate1.EED > exRate2.BED)))
												

RETURN 
END