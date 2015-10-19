﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_Rule_GetByType] 
	@TypeID INT
AS
BEGIN
	SELECT r.ID,
		   r.TypeID,
		   r.RuleDetails  ,
		   r.BED,
		   r.EED
	FROM [rules].[Rule] r 
	WHERE r.TypeID=@TypeID

END