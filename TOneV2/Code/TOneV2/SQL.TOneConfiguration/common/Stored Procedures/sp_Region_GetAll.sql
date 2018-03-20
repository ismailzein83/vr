CREATE PROCEDURE [common].[sp_Region_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	c.ID,c.Name,c.CountryId,c.Settings, c.CreatedTime, c.CreatedBy, c.LastModifiedBy, c.LastModifiedTime
	FROM	[common].Region  as c WITH(NOLOCK) 
END