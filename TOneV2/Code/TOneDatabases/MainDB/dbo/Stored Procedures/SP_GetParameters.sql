﻿
CREATE PROCEDURE [dbo].[SP_GetParameters]
WITH RECOMPILE
AS
BEGIN	
	SELECT *
	FROM dbo.SystemParameter WITH(NOLOCK)
END