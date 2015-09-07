-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CurrencyExchangeRate_GetByExchangeDate]
	-- Add the parameters for the stored procedure here
	@Date Date
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT	c.CurrencyExchangeRateID,
			c.CurrencyID,
			c.Rate,
			c.ExchangeDate 
	FROM	CurrencyExchangeRate c WITH (NOLOCK) 
	WHERE	 c.ExchangeDate<=@Date
END