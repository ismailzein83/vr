﻿-- =============================================
-- Author:		Rabih Fashwal
-- Create date: 02/25/2015
-- Description:	Get Supplier Codes with Zone according to CodePrefixGroup defined in LCR.RootCodePrefix
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Code_GetByCodePrefixGroupForActiveSuppliers]
	@CodePrefixGroup VARCHAR(6),
	@ActiveSuppliersInfo BEntity.CarrierAccountInfoType READONLY,
	@EffectiveTime datetime,
	@IsFuture bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    SELECT c.[ID]
		  ,c.[Code]
		  ,c.[ZoneID]
		  ,c.[BeginEffectiveDate]
		  ,c.[EndEffectiveDate]
		  ,c.[IsEffective]
		  ,c.[UserID]
		  ,c.[timestamp]
		  ,z.CodeGroup 
		  ,z.SupplierID
    FROM Code C WITH(NOLOCK)
    JOIN Zone Z WITH(NOLOCK) ON  C.ZoneID = Z.ZoneID 
    JOIN @ActiveSuppliersInfo sup ON Z.SupplierID = sup.CarrierAccountID
    WHERE   C.Code like @CodePrefixGroup + '%'   
			AND 
			(
				(@IsFuture = 0 AND C.BeginEffectiveDate <= @EffectiveTime AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > @EffectiveTime))
				OR
				(@IsFuture = 1 AND (C.BeginEffectiveDate > GETDATE() OR C.EndEffectiveDate IS NULL))
			)
			AND
			(
				(@IsFuture = 0 AND Z.BeginEffectiveDate <= @EffectiveTime AND (Z.EndEffectiveDate IS NULL OR Z.EndEffectiveDate > @EffectiveTime))
				 OR
				(@IsFuture = 1 AND (Z.BeginEffectiveDate > GETDATE() OR Z.EndEffectiveDate IS NULL))
			)
	ORDER BY C.Code
END