﻿CREATE PROCEDURE [common].[sp_City_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	c.ID,
			c.Name,
			c.CountryId
	FROM	[common].City  as c WITH(NOLOCK) 
END