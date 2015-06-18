





CREATE PROCEDURE [Analytics].[SP_Billing_CustomerServices](
	@FromDate Datetime ,
	@ToDate Datetime
)
with Recompile
	AS 
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

BEGIN
DECLARE @NumberOfDays INT 
SET @NumberOfDays = DATEDIFF(dd,@FromDate,@ToDate)

SELECT ca.CarrierAccountID AS AccountID, ((isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ER.Rate, 1))*@NumberOfDays AS Services
FROM CarrierAccount ca 
JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
LEFT JOIN @ExchangeRates ER ON ER.Currency = cp.CurrencyID AND ER.Date = @FromDate
END

RETURN