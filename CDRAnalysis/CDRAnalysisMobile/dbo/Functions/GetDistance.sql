CREATE FUNCTION [dbo].[GetDistance]
(
	@P1Lat numeric(18,9),
	@P1Long numeric(18,9),
	@P2Lat numeric(18,9),
	@P2Long numeric(18,9)
)
RETURNS numeric(18,9)
AS
BEGIN
	 
	declare @geo1 geography = geography::Point(@P1Lat, @P1Long, 4326),
            @geo2 geography = geography::Point(@P2Lat, @P2Long, 4326)
    RETURN @geo1.STDistance(@geo2)


END


/*

select [dbo].[GetDistance] (33.33466000, 44.31161000, 33.29902000, 44.33897000)

*/