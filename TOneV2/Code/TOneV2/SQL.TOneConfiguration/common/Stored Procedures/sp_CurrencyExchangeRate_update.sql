-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_CurrencyExchangeRate_update]
    @ID bigint,
    @Rate decimal(20,10),
	@CurrencyID int ,
	@ExchangeDate datetime
	
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.CurrencyExchangeRate WHERE ID != @ID AND ExchangeDate = @ExchangeDate and CurrencyID= @CurrencyID)
	BEGIN
	UPDATE common.CurrencyExchangeRate
	SET Rate = @Rate , CurrencyID = @CurrencyID , ExchangeDate = @ExchangeDate,LastModifiedTime=getdate()
	WHERE ID = @ID
	END
END