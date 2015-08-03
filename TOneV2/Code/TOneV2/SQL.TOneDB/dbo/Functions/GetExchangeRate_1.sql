CREATE  FUNCTION [dbo].[GetExchangeRate](@CurrencyID varchar(3),@Date Datetime)
RETURNS float
AS
BEGIN 
	DECLARE @rate float
	SELECT @rate = (
	SELECT TOP 1 cer.Rate FROM CurrencyExchangeRate cer 
	WHERE cer.CurrencyID = @CurrencyID AND cer.ExchangeDate <= @Date 
	ORDER BY cer.ExchangeDate DESC)
	
	RETURN isnull(@rate,1) 
END