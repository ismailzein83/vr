CREATE PROCEDURE [TOneWhS_BE].[sp_FinancialAccount_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT  fa.ID,
			fa.CarrierAccountId,
			fa.CarrierProfileID,
			fa.FinancialAccountDefinitionId,
			fa.EED,
			fa.BED,
			fa.FinancialAccountSettings,
			fa.CreatedTime,
			fa.CreatedBy,
			fa.LastModifiedBy,
			fa.LastModifiedTime
	FROM    [TOneWhS_BE].FinancialAccount fa WITH(NOLOCK)
	SET NOCOUNT OFF
END