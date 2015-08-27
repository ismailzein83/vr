-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierAccount_Update] 
(
@AccountType tinyint,
@ActivationStatus tinyint,
@CarrierAccountId varchar(4),
@CustomerPaymentType tinyint,
@NameSuffix nvarchar(100),
@ProfileCompanyName nvarchar(200),
@ProfileId smallint,
@ProfileName nvarchar(200),
@RoutingStatus tinyint,
@SupplierPaymentType tinyint,
@CarrierMaskId int
)
AS
BEGIN
DECLARE @TranName VARCHAR(20);
SELECT @TranName = 'MyTransaction';

BEGIN TRANSACTION @TranName
UPDATE CarrierAccount
SET AccountType = @AccountType,
	ActivationStatus = @ActivationStatus,
	CarrierAccountID = @CarrierAccountId,
	CustomerPaymentType = @CustomerPaymentType,
	NameSuffix = @NameSuffix,
	ProfileID = @ProfileId,
	RoutingStatus = @RoutingStatus,
	SupplierPaymentType = @SupplierPaymentType,
	CarrierMaskId = @CarrierMaskId
WHERE CarrierAccountID = @CarrierAccountId

UPDATE CarrierProfile
SET Name = @ProfileName,
	CompanyName = @ProfileCompanyName
WHERE ProfileID = @ProfileId
COMMIT TRANSACTION @TranName

END