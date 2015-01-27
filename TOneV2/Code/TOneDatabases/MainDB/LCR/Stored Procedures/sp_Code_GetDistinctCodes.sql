-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_Code_GetDistinctCodes]
	@ActiveSuppliersCodeInfo LCR.SuppliersCodeInfoType READONLY,
	@EffectiveOn datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT distinct c.Code
    FROM Code C WITH(NOLOCK)
    JOIN Zone Z WITH(NOLOCK) ON Z.ZoneID = C.ZoneID
    JOIN @ActiveSuppliersCodeInfo sup ON Z.SupplierID = sup.SupplierID
    WHERE 
		C.BeginEffectiveDate <= @EffectiveOn
			AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > @EffectiveOn)
END