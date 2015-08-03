
CREATE Proc [dbo].[UpdateCarrierAccountFromPortal]
(
		
       @CarrierAccountID varchar(5)
      ,@ActivationStatus tinyint 
      ,@Notes text
      ,@CustomerCreditLimit int
      ,@CustomerGMTTime smallint
)
as

		UPDATE CarrierAccount
		SET
			     ActivationStatus=@ActivationStatus
				,Notes = @Notes
				,CustomerCreditLimit = @CustomerCreditLimit
				,CustomerGMTTime=@CustomerGMTTime
		WHERE CarrierAccountID = @CarrierAccountID
		
	RETURN	SELECT profileID FROM CarrierAccount ca WHERE ca.CarrierAccountID = @CarrierAccountID