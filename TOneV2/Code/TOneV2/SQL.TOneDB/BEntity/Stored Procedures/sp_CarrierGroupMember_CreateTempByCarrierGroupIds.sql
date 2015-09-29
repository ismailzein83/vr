-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierGroupMember_CreateTempByCarrierGroupIds]
	@TempTableName VARCHAR(200) = NULL,
	@CarrierGroupIds varchar(max) ,
	@AssignedCarriers varchar(max)
AS
BEGIN

	SET NOCOUNT ON;
		
    IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			IF(@CarrierGroupIds IS NOT NULL)
				BEGIN
				DECLARE @CarrierGroupIdsTable TABLE (CarrierGroupId int)
				INSERT INTO @CarrierGroupIdsTable (CarrierGroupId)
				select Convert(int, ParsedString) from [BEntity].[ParseStringList](@CarrierGroupIds)
			END
			IF(@AssignedCarriers IS NOT NULL)
			BEGIN
				DECLARE @AssignedCarriersTable TABLE (CarrierId varchar(5))
				INSERT INTO @AssignedCarriersTable (CarrierId)
				select Convert(varchar, ParsedString) from [BEntity].[ParseStringList](@AssignedCarriers)
			END
			
			;WITH CarrierAccountIDs (CarrierAccountID)
			AS
			(
				SELECT DISTINCT cgm.CarrierAccountID
				FROM BEntity.CarrierGroupMember as cgm
				JOIN @CarrierGroupIdsTable as ids ON ids.CarrierGroupId = cgm.CarrierGroupID
			) , AllCarrierAccounts AS (
		    
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
						'' as CarrierAccountName,
						ca.IsCustomerCeiling,
						ca.IsSupplierCeiling
			FROM CarrierAccount  as ca WITH(NOLOCK) Join CarrierAccountIDs as caIds on ca.CarrierAccountID = caIds.CarrierAccountID
										INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
										WHERE 
										((@AssignedCarriers IS NULL)
										Or (ca.CarrierAccountId IN(SELECT CarrierID from @AssignedCarriersTable)))

			)
			
			SELECT *
			INTO #RESULT
			FROM AllCarrierAccounts
			
										
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

	SET NOCOUNT OFF
END