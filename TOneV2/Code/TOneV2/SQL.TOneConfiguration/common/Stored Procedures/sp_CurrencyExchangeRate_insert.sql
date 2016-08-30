-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_CurrencyExchangeRate_insert]
    @Rate decimal(20,10),
	@CurrencyID int,
	@ExchangeDate datetime,
	@id INT OUT
AS
BEGIN

	INSERT INTO common.CurrencyExchangeRate(Rate,CurrencyID,ExchangeDate)
	VALUES (@Rate,@CurrencyID,@ExchangeDate)
	
	SET @id = SCOPE_IDENTITY()
END