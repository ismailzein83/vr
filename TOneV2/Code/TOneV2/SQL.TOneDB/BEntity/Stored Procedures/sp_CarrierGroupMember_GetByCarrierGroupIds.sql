-- =============================================
-- Author:		Hadi Hawi
-- Create date: 08/07/2015
-- Description:	
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierGroupMember_GetByCarrierGroupIds]
	@CarrierGroupIds BEntity.MemberIdType READONLY
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	WITH CarrierAccountIDs (CarrierAccountID)

AS
-- Define the CTE query.
(
    SELECT DISTINCT cgm.CarrierAccountID
    FROM BEntity.CarrierGroupMember as cgm
    JOIN @CarrierGroupIds as ids ON ids.ID = cgm.CarrierGroupID
)
-- Define the outer query referencing the CTE name.
SELECT ca.CarrierAccountId,
			cp.ProfileId ,
			cp.Name AS ProfileName,
			cp.CompanyName AS ProfileCompanyName,
			ca.ActivationStatus,
			ca.RoutingStatus,
			ca.AccountType,
			ca.CustomerPaymentType,
			ca.SupplierPaymentType,
			ca.NameSuffix
FROM CarrierAccount  as ca WITH(NOLOCK) Join CarrierAccountIDs AS caIds ON ca.CarrierAccountID = caIds.CarrierAccountID
										INNER JOIN CarrierProfile cp ON ca.ProfileID = cp.ProfileID
										
										
										
										
										
END