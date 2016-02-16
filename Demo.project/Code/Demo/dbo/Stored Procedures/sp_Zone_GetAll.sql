
CREATE PROCEDURE [dbo].[sp_Zone_GetAll] 
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [ID],
		[CountryID],
		[Name],
		[BED],
		[EED]
	FROM [dbo].[Zone] sz
END