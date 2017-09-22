

CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierCode_Preview_GetNumberOfCodesWithChangeType]
	@ProcessInstanceID_IN INT,
	@CodeChangeType_IN INT

AS
BEGIN

	SELECT	COUNT(ChangeType)
	FROM	[TOneWhS_SPL].[SupplierCode_Preview] WITH(NOLOCK)
	WHERE	ProcessInstanceID = @ProcessInstanceID_IN and ChangeType=@CodeChangeType_IN
	
END