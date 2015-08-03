
CREATE Proc [dbo].[DeleteCarrierAndProfile]
(
	@CarrierID VARCHAR(5)
	
)
as
begin
	
	DECLARE @ProfileID INT 
	   SELECT @ProfileID = ProfileID FROM CarrierAccount ca WHERE ca.CarrierAccountID = @CarrierID
	
	
	DELETE FROM CarrierAccount WHERE CarrierAccountID = @CarrierID
	DELETE FROM CarrierProfile WHERE ProfileID = @ProfileID
		
END