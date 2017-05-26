﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierAccount_Insert]
	@Name nvarchar(255),
	@CarrierProfileId int,
	@AccountType int,
	@SellingNumberPlanID int = null,
	@SellingProductID int = null,
	@CustomerSettings nvarchar(max),
	@SupplierSettings nvarchar(max),
	@CarrierAccountSettings nvarchar(max),	
	@Id int out
AS
BEGIN
	set @id = 0;
	if not exists(select 1 from TOneWhS_BE.CarrierAccount where [NameSuffix] = @Name and [CarrierProfileID] = @CarrierProfileId)
	begin
		insert into TOneWhS_BE.CarrierAccount([NameSuffix], [CarrierProfileID], [AccountType], [SupplierSettings], [CustomerSettings], [CarrierAccountSettings], [SellingNumberPlanID], [SellingProductID])
		values (@Name, @CarrierProfileId, @AccountType, @SupplierSettings, @CustomerSettings, @CarrierAccountSettings, @SellingNumberPlanID, @SellingProductId)
		set @Id = scope_identity()
	end
END