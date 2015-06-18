-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierAccount_GetByName]
	 (@Name VARCHAR(30) =  NULL,
	 @CompanyName VARCHAR(50) = NULL,
	 @From int = 0,
	 @To int = 50)
AS
BEGIN
	SET NOCOUNT ON;
;WITH CarrierAccountData AS
(
SELECT ca.CarrierAccountId,
	cp.ProfileId ,
	cp.Name AS ProfileName,
	cp.CompanyName AS ProfileCompanyName,
	ca.ActivationStatus,
	ca.RoutingStatus,
	ca.AccountType,
	ca.CustomerPaymentType,
	ca.SupplierPaymentType,
	ca.NameSuffix,
	ROW_NUMBER() OVER(ORDER BY ca.CarrierAccountId ASC) as  RowNumber
FROM CarrierAccount ca
		INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
			WHERE 
			(@Name IS NULL OR cp.Name = @Name)
		AND (@CompanyName IS NULL OR cp.CompanyName = @CompanyName)
		)
		SELECT * from CarrierAccountData ca
		WHERE ca.RowNumber BETWEEN @From AND @To

END