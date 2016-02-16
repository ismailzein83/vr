CREATE PROCEDURE [dbo].[sp_OperatorAccount_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT ca.ID,
		ca.OperatorProfileID,
		ca.NameSuffix,
		ca.CustomerSettings,
		ca.SupplierSettings,
		ca.OperatorAccountSettings
	FROM dbo.OperatorAccount ca
	SET NOCOUNT OFF
END