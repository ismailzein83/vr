
CREATE PROCEDURE [TOneWhS_Deal].[sp_DaysToReprocess_Insert]
	@Date Date,
	@IsSale bit,
	@CarrierAccountId int
AS
BEGIN
	INSERT into [TOneWhS_Deal].[DaysToReprocess] ([Date],[IsSale],[CarrierAccountId]) 
	Values (@Date,@IsSale,@CarrierAccountId)
END