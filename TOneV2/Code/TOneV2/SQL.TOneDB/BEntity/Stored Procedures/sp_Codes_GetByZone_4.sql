-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Codes_GetByZone]
	@ZoneID int,
	@EffectiveOn DateTime
AS
BEGIN
	SET NOCOUNT ON;
		SELECT c.ID, c.Code, c.ZoneID, c.BeginEffectiveDate, c.EndEffectiveDate, z.CodeGroup, z.SupplierID, c.[timestamp]
		FROM code c with(nolock) 
		INNER JOIN Zone z with(nolock) on c.zoneID= z.zoneID
		WHERE c.BeginEffectiveDate <= @EffectiveOn  AND 
		(c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > @EffectiveOn ) AND
		 c.ZoneID = @ZoneID
		 ORDER BY c.ZoneID, Code ASC

END