-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_Code_GetForActiveSuppliers]
	@ActiveSuppliersCodeInfo LCR.SuppliersCodeInfoType READONLY,
	@EffectiveOn datetime,
	@OnlySuppliersWithUpdateCodes bit
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
    JOIN @ActiveSuppliersCodeInfo sup ON Z.SupplierID = sup.SupplierID
    WHERE      
		(@OnlySuppliersWithUpdateCodes = 0 OR sup.HasUpdatedCodes = 1)
		AND C.BeginEffectiveDate <= @EffectiveOn
			AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > @EffectiveOn)
	ORDER BY Z.SupplierID, C.Code desc
END