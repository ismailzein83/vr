CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierAccount_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT
			ca.ID,
			ca.AccountType,
			ca.CarrierProfileID,
		    ca.Name,
			ca.CustomerSettings,
			ca.SupplierSettings,
			cp.Name as CarrierProfileName
	FROM TOneWhS_BE.CarrierAccount ca  
	Join TOneWhS_BE.CarrierProfile cp ON ca.CarrierProfileID=cp.ID                         

		
	SET NOCOUNT OFF
END