CREATE PROCEDURE [dbo].[bp_CleanEndedZonesRelatedEntities]

AS

DELETE c from  commission c with (NOLOCK) where ISEFFECTIVE='y' 
AND not exists(Select ZONEID FROM ZONE WITH (NOLOCK) where Zone.Zoneid=C.zoneid)

DELETE c from  TODCONSIDERATION c with (NOLOCK) where ISEFFECTIVE='y' 
AND not exists(Select ZONEID FROM ZONE WITH (NOLOCK) where Zone.Zoneid=C.zoneid)

DELETE c from  TARIFF c with (NOLOCK) where ISEFFECTIVE='y' 
AND not exists(Select ZONEID FROM ZONE WITH (NOLOCK) where Zone.Zoneid=C.zoneid)

DELETE c from  RATE c with (NOLOCK) where ISEFFECTIVE='y' 
AND not exists(Select ZONEID FROM ZONE WITH (NOLOCK) where Zone.Zoneid=C.zoneid)

DELETE c from  CODE c with (NOLOCK) where ISEFFECTIVE='y' 
AND not exists(Select ZONEID FROM ZONE WITH (NOLOCK) where Zone.Zoneid=C.zoneid)

DELETE c from  RouteBlock c with (NOLOCK) where ISEFFECTIVE='y' 
AND not exists(Select ZONEID FROM ZONE WITH (NOLOCK) where Zone.Zoneid=C.zoneid)

DELETE c from  RouteOverride c with (NOLOCK) where ISEFFECTIVE='y' 
AND not exists(Select ZONEID FROM ZONE WITH (NOLOCK) where Zone.Zoneid=C.OurZoneID)