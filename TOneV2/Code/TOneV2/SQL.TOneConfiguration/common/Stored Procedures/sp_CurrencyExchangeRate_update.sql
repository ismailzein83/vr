-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_CurrencyExchangeRate_update]
    @ID bigint,
    @Rate decimal(18,5),
	@CurrencyID int ,
	@ExchangeDate datetime
	
	
AS
BEGIN
	UPDATE common.CurrencyExchangeRate
	SET Rate = @Rate , CurrencyID = @CurrencyID , ExchangeDate = @ExchangeDate
	WHERE ID = @ID
END