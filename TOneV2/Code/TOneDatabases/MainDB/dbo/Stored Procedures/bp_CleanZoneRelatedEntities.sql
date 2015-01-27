CREATE PROCEDURE [dbo].[bp_CleanZoneRelatedEntities]
(
	@ZoneIDArray varchar(8000) = NULL ,
	@EndEffectiveDate datetime  
)
AS
-----------------------------
-- Clean RouteOverride
-----------------------------
SET NOCOUNT ON 

Update ToDConsideration Set EndEffectiveDate=@EndEffectiveDate
from ToDConsideration 
Where ZoneID	in (SELECT * FROM [dbo].[ParseArray] (@ZoneIDArray,','))


Update Tariff Set EndEffectiveDate=@EndEffectiveDate
from Tariff 
Where ZoneID	in (SELECT * FROM [dbo].[ParseArray] (@ZoneIDArray,','))

Update Commission Set EndEffectiveDate=@EndEffectiveDate
from Commission 
Where ZoneID	in (SELECT * FROM [dbo].[ParseArray] (@ZoneIDArray,','))

Update RouteOverride Set EndEffectiveDate=@EndEffectiveDate
from RouteOverride 
Where OurZoneID	in (SELECT * FROM [dbo].[ParseArray] (@ZoneIDArray,','))