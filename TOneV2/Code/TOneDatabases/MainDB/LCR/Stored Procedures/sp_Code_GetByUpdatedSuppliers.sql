-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_Code_GetByUpdatedSuppliers]
	@codeUpdatedAfter timestamp,
	@EffectiveOn datetime
AS
BEGIN
	WITH suppliersWithUpdatedCodes AS
	(	
		SELECT distinct z.SupplierID
		FROM Code c WITH (NOLOCK)
		JOIN Zone z WITH (NOLOCK) ON c.ZoneID = z.ZoneID
		WHERE c.timestamp > @codeUpdatedAfter
	)

SELECT 
       c.[ID]
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
    JOIN Zone Z WITH(NOLOCK) ON C.ZoneID = Z.ZoneID 
    JOIN suppliersWithUpdatedCodes s ON Z.SupplierID = s.SupplierID
    WHERE (C.EndEffectiveDate IS NULL OR (C.EndEffectiveDate > @EffectiveOn And C.BeginEffectiveDate<>C.EndEffectiveDate)) 
      AND (Z.EndEffectiveDate IS NULL OR (Z.EndEffectiveDate > @EffectiveOn And Z.BeginEffectiveDate<>Z.EndEffectiveDate)) 
    ORDER By Z.SupplierID
END