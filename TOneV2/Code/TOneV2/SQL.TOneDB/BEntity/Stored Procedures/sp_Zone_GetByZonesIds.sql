-- =============================================
-- Author:		Ali Ballouk
-- Create date: 03/18/2015
-- Description:	Get Zones info using list of Zones ids
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Zone_GetByZonesIds]
	@ZonesIds BEntity.IntIDType READONLY	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    SELECT Z.ZoneID
		  ,Z.Name
    FROM Zone Z WITH(NOLOCK)    
    JOIN @ZonesIds as ids ON ids.ID = Z.ZoneID
END