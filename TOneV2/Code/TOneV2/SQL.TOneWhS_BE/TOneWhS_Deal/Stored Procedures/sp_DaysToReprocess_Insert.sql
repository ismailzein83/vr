﻿
CREATE PROCEDURE [TOneWhS_Deal].[sp_DaysToReprocess_Insert]
	@Date Date,
	@IsSale bit,
	@CarrierAccountId int,
	@Id int out
AS
BEGIN
	IF  NOT EXISTS( SELECT * FROM [TOneWhS_Deal].[DaysToReprocess] WHERE @Date=[Date] AND @IsSale=[IsSale] AND @CarrierAccountId=[CarrierAccountId] )
	BEGIN
	INSERT into [TOneWhS_Deal].[DaysToReprocess] ([Date],[IsSale],[CarrierAccountId]) 
	Values (@Date,@IsSale,@CarrierAccountId)
	Set @Id = SCOPE_IDENTITY()
	END
END