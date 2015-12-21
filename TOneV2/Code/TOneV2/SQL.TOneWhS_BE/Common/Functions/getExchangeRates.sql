-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [Common].[getExchangeRates](@From DateTime, @To Datetime) 
RETURNS @ExchangeRates Table  
					(
						CurrencyID int NOT NULL,
						Rate Decimal(18,6) NOT NULL,
						BED DATETIME NOT NULL,
						EED DATETIME
					) 
AS
BEGIN
	with CTE AS (
		SELECT cr1.[CurrencyID]
			  ,cr1.[Rate]
			  ,cr1.[ExchangeDate] BED
			  ,cr2.[ExchangeDate] EED
		  FROM Common.[CurrencyExchangeRate] cr1
		  LEFT JOIN Common.[CurrencyExchangeRate] cr2 ON	cr1.CurrencyID = cr2.CurrencyID 
															AND cr1.ExchangeDate < cr2.ExchangeDate 
															AND (cr2.[ExchangeDate] <= @To OR @To IS NULL)
		  WHERE cr1.[ExchangeDate] >= @From 
		  AND (cr1.[ExchangeDate] <= @To OR @To IS NULL) 
	  )
	  
	  INSERT INTO @ExchangeRates
	  SELECT [CurrencyID], [Rate], BED, MIN(EED) EED  FROM CTE
	  GROUP BY [CurrencyID], [Rate], BED
RETURN 
END