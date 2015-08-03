
CREATE Proc [dbo].[SaveCarrierAccountFromPortal]
(
		
       @CarrierAccountID varchar(5)
      ,@ProfileID smallint
      ,@ActivationStatus tinyint 
      ,@Notes text
      ,@UserID INT
      ,@CustomerCreditLimit int
      ,@CustomerGMTTime SMALLINT
      ,@ServiceFlag SMALLINT
      ,@RoutingStatus TINYINT
      ,@AccountType TINYINT
      ,@PaymentType TINYINT
)
as
BEGIN
	DECLARE @TempGMTTime SMALLINT SET @TempGMTTime = @CustomerGMTTime
		
		IF not EXISTS (SELECT ctzi.BaseUtcOffset FROM CustomTimeZoneInfo ctzi WHERE ctzi.BaseUtcOffset = @CustomerGMTTime)
		BEGIN
		set @TempGMTTime = 0
			END	
		
					insert into [CarrierAccount] (ProfileID,CarrierAccountID,ActivationStatus,Notes,UserID,CustomerCreditLimit,CustomerGMTTime,ServicesFlag,RoutingStatus,AccountType,CustomerPaymentType,IsDeleted,IsPassThroughCustomer,IsProduct,SupplierPaymentType,SupplierCreditLimit,NameSuffix,CustomerActivateDate)
					VALUES(@ProfileID,@CarrierAccountID,0,@Notes,1,@CustomerCreditLimit,@TempGMTTime,@ServiceFlag,@RoutingStatus,0,@PaymentType,'N','Y','N',0,0,'Zebra',GETDATE())
					return SCOPE_IDENTITY()--Insertion done succsessful
				end