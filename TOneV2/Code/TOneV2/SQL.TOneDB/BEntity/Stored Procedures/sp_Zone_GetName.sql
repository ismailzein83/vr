
CREATE PROCEDURE [BEntity].[sp_Zone_GetName]
	@ZoneId INT
AS
BEGIN

SELECT z.Name FROM Zone z WITH (NOLOCK)
WHERE z.ZoneID = @ZoneId

END