﻿
Create PROCEDURE [reprocess].[sp_ReprocessDefinition_GetAll]
AS
BEGIN
	SELECT	ID, Name, Settings
	FROM	[reprocess].ReprocessDefinition
END