CREATE PROCEDURE [TOneWhS_AccBalance].[sp_CarrierFinancialAccount_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT cfa.ID,
		cfa.CarrierAccountId,
		cfa.CarrierProfileID,
		cfa.EED,
		cfa.BED,
		cfa.FinancialAccountSettings
	FROM [TOneWhS_AccBalance].CarrierFinancialAccount cfa WITH(NOLOCK)
	SET NOCOUNT OFF
END