CREATE PROCEDURE [common].[sp_CurrencyExchangeRate_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	[ID],[Rate],[CurrencyID],[ExchangeDate],[SourceID]
    from	[common].CurrencyExchangeRate WITH(NOLOCK) 
END