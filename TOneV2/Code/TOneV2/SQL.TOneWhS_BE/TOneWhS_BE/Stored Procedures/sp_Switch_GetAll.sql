﻿CREATE PROCEDURE [TOneWhS_BE].[sp_Switch_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	s.ID,
			s.Name
	FROM	[TOneWhS_BE].Switch  as s WITH(NOLOCK) 
END